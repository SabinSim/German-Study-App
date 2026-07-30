using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Infrastructure;

public class OpenAiAnalysisService : IGermanAnalysisService
{

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiAnalysisService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }
    
    public async Task<AnalyzedSentence> AnalyzeAsync(string germanText, TargetLanguage targetlanguage, CancellationToken ct = default)
    {
        string systemPrompt = """
                              너는 독일어 문법 선생님이야. 주어진 독일어 문장을 분석해줘.
                              - 동사는 원형, 과거형, 과거분사형을 알려주고, 규칙/불규칙 여부도 설명해.
                              - 분리동사면 왜 분리됐는지 설명해.
                              - 명사는 관사가 왜 그런지 설명해.
                              - 형용사는 어미 변화 이유를 설명해.
                              - 자연스러운 번역이랑 직역(직독직해) 둘 다 알려줘.
                              """;
        
        var result = new AnalyzedSentence();
        return result;
    }
}

