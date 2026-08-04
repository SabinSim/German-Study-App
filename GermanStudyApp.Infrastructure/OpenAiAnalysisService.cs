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
        _httpClient = httpClient;
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }
    
    public async Task<AnalyzedSentence> AnalyzeAsync(string germanText, TargetLanguage targetlanguage, CancellationToken ct = default)
    {
        string languageInstruction;

        if (targetlanguage == TargetLanguage.Korean)
        {
            languageInstruction = "Translate Korean.";
        }
        else
        {
            languageInstruction = "Translate English.";
        }
        
        string systemPrompt = $$"""
                                너는 독일어 문법 선생님이야. 주어진 독일어 문장을 분석해줘.
                                - 동사는 원형, 과거형, 과거분사형을 알려주고, 규칙/불규칙 여부도 설명해.
                                - 분리동사면 왜 분리됐는지 설명해.
                                - 명사는 관사가 왜 그런지 설명해.
                                - 형용사는 어미 변화 이유를 설명해.
                                - 자연스러운 번역이랑 직역(직독직해) 둘 다 알려줘.

                                {{languageInstruction}}


                                반드시 아래 JSON 형식으로만 답해, 다른 텍스트는 절대 추가하지 마:
                                {
                                  "Translation": "자연스러운 번역",
                                  "LiteralTranslation": "직역",
                                  "WordAnalyses": [
                                    {
                                      "Word": "단어",
                                      "Gender": "성별/관사 설명",
                                      "OriginalWord": "동사 원형",
                                      "PastParticiple": "과거분사형",
                                      "GrammarExplanation": "문법 설명"
                                    }
                                  ]
                                }
                                """;
        
        var requestBody = new
        {
            model = "gpt-4o-mini",
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = germanText }
            }
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
        
        
        var result = JsonSerializer.Deserialize<AnalyzedSentence>(content);
        return result;
        
    }
}
