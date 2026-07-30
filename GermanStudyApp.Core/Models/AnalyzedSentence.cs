namespace GermanStudyApp.Core.Models;

public class AnalyzedSentence
{
    public string Translation { get; set; }
    public string LiteralTranslation { get; set; }
    public List<WordAnalysis> WordAnalyses { get; set; }
}