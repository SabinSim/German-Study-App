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
        var result = new AnalyzedSentence();
        return result;
    }
}

https://github.com/SabinSim/German-Study-App.git