using System.Net.Http.Json;
using System.Text.Json;
using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;
using System.Net.Http.Headers;

namespace GermanStudyApp.Infrastructure;

public class OpenAiAnalysisService : IGermanAnalysisService
{

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiAnalysisService(HttpClient httpClient, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));
        }
        
        _httpClient = httpClient;
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }
    
    public async Task<AnalyzedSentence> AnalyzeAsync(string germanText, TargetLanguage targetlanguage, CancellationToken ct = default)
    {
        var languageName = targetlanguage == TargetLanguage.Korean ? "Korean" : "English";

        // 영어 결과에 한글이 섞여 나오는 경우를 줄이기 위해, 1회 재시도(더 강한 지시)를 허용한다.
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var strictLanguageRule = attempt == 0
                ? $"All text fields in the JSON must be written in {languageName}."
                : $"CRITICAL: Every text field in the JSON must be written in {languageName} only. Do not use any other language.";

            var systemPrompt = $$"""
                                 you are a German grammar teacher. Please analyze the given German sentence.
                                 - Please provide the infinitive, past tense, and past participle forms of the verbs, and explain whether they are regular or irregular.
                                 - If the verb is separable, explain why it is separated.
                                 - For nouns, explain why the article is as it is.
                                 - For adjectives, explain the reason for the ending changes.
                                 - Provide both a natural translation and a literal translation.
                                 - For each word, provide its meaning/translation.

                                 {{strictLanguageRule}}

                                 you must respond only in the following JSON format, and do not add any other text:
                                 {
                                   "Translation": "<natural translation>",
                                   "LiteralTranslation": "<literal, word-by-word translation>",
                                   "WordAnalyses": [
                                   {
                                     "Word": "<word>",
                                     "Meaning": "<meaning/translation of this word>",
                                     "Gender": "<gender/article explanation>",
                                     "OriginalWord": "<infinitive form of the verb>",
                                     "PastParticiple": "<past participle form>",
                                     "GrammarExplanation": "<grammar explanation>"
                                     }
                                   ]
                                 }
                                 """;

            var content = await RequestCompletionContentAsync(systemPrompt, germanText, true, ct);

            var result = JsonSerializer.Deserialize<AnalyzedSentence>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (result is null)
            {
                throw new InvalidOperationException("AI response could not be parsed into AnalyzedSentence.");
            }

            if (targetlanguage == TargetLanguage.English && attempt == 0 && ContainsHangulInResult(result))
            {
                continue;
            }

            return result;
        }

        throw new InvalidOperationException("AI response did not satisfy the requested output language.");
    }

    public async Task<string> GenerateExampleSentenceAsync(string word, GermanLevel level,
        CancellationToken ct = default)
    {
        var systemPrompt = $"""
                            You are a German teacher. Write exactly one natural German example sentence
                            that uses the word "{word}", at CEFR level {level}.
                            Respond only with the German sentence, without any additional text or explanation.
                            """;
        var content = await RequestCompletionContentAsync(systemPrompt, word, false, ct);
        
        return content.Trim();
    }

    private async Task<string> RequestCompletionContentAsync(string systemPrompt, string germanText, bool requireJson, CancellationToken ct)
    {
        var requestBody = new
        {
            model = "gpt-4o-mini",
            response_format =  requireJson ? new { type = "json_object" } : null,
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = germanText },
            },
        };

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.openai.com/v1/chat/completions",
            requestBody,
            ct);

        response.EnsureSuccessStatusCode();

        var openAiResponse = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);

        var content = openAiResponse
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content ?? throw new InvalidOperationException("AI response content was empty.");
    }

    private static bool ContainsHangulInResult(AnalyzedSentence result)
    {
        if (ContainsHangul(result.Translation) || ContainsHangul(result.LiteralTranslation))
        {
            return true;
        }

        foreach (var item in result.WordAnalyses)
        {
            if (ContainsHangul(item.Word) ||
                ContainsHangul(item.Meaning) ||
                ContainsHangul(item.Gender) ||
                ContainsHangul(item.OriginalWord) ||
                ContainsHangul(item.PastParticiple) ||
                ContainsHangul(item.GrammarExplanation))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsHangul(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        foreach (var ch in text)
        {
            if (ch >= '\uAC00' && ch <= '\uD7AF')
            {
                return true;
            }
        }

        return false;
    }
}
