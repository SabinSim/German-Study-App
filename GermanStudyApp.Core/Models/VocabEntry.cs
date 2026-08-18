namespace GermanStudyApp.Core.Models;

public class VocabEntry
{
    public int Id { get; set; }
    public required string Word { get; set; }
    public required string Meaning { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime NextReviewDate { get; set; }
    public int BoxLevel { get; set; }
    public required int DeckId { get; set; }
}