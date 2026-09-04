using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Core.Interfaces;

public interface IGermanAnalysisService
{
    Task<AnalyzedSentence> AnalyzeAsync(
        string germanText,
        TargetLanguage targetLanguage,
        CancellationToken ct = default);
    
    Task<string> GenerateExampleSentenceAsync(
        string word,
        GermanLevel level,
        CancellationToken ct = default);
}