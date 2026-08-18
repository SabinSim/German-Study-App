namespace GermanStudyApp.Core.Models;

public class Deck
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? ParentDeckId { get; set; }
    
}