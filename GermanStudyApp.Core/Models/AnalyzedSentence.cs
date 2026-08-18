namespace GermanStudyApp.Core.Models;

public class AnalyzedSentence
{
    public required string Translation { get; set; }
    public required string LiteralTranslation { get; set; }
    public List<WordAnalysis> WordAnalyses { get; set; } = new();
}