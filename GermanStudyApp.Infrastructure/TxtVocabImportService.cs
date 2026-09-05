using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;
namespace GermanStudyApp.Infrastructure;

public class TxtVocabImportService : IVocabImportService
{
    public List<VocabEntry> ParseTxt(string fileContent, int deckId)
    {
        string[] lines = fileContent.Split('\n');
        
        var entries = new List<VocabEntry>();

        foreach (var line in lines)
        {
            var parts = line.Split(',');

            if (string.IsNullOrWhiteSpace(line) || parts.Length < 2)
            {
                continue;
            }

            var entry = new VocabEntry()
            {
                Word = parts[0].Trim(),
                Meaning = parts[1].Trim(),
                DeckId = deckId,
                DateAdded = DateTime.Now,
                NextReviewDate = DateTime.Now,
                BoxLevel = 1
            };
            
            entries.Add(entry);
        }
        return entries;
    }
}