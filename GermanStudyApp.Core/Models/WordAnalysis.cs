namespace GermanStudyApp.Core.Models;

public class WordAnalysis
{
    public required string Word { get; set; }
    public required string Meaning { get; set; }
    public string? Gender { get; set; }
    public string? OriginalWord { get; set; }
    public string? PastParticiple { get; set; }
    public string? GrammarExplanation { get; set; }
    
}