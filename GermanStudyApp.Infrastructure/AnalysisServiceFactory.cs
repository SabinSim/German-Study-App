using System.Net.Http;
using GermanStudyApp.Core.Interfaces;

namespace GermanStudyApp.Infrastructure;

// 환경 변수에서 API 키를 읽어 OpenAiAnalysisService를 만들어주는 공용 헬퍼.
// 여러 ViewModel과 VocabRepository가 각자 API 키 읽는 코드를 반복하지 않도록 모아둔 곳.
public static class AnalysisServiceFactory
{
    public static IGermanAnalysisService Create()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = "DUMMY_KEY_NOT_SET";
        }

        return new OpenAiAnalysisService(new HttpClient(), apiKey);
    }
}
