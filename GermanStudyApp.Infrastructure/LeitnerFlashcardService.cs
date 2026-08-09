using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Infrastructure;

public class LeitnerFlashcardService : IFlashcardService
{
    public void ApplyReviewResult(VocabEntry entry, bool wasCorrect)
    {
        if (wasCorrect)
        {
            entry.BoxLevel += 1;
            
            if (entry.BoxLevel > 5)
            {
                entry.BoxLevel = 5;
            }
        }
        else
        {
            entry.BoxLevel = 1;
        }
        
        entry.NextReviewDate =  DateTime.Now.AddDays(entry.BoxLevel);
    }
}