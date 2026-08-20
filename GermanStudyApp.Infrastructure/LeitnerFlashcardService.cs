using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Infrastructure;

public class LeitnerFlashcardService : IFlashcardService
{
    public void ApplyReviewResult(VocabEntry entry, ReviewRating rating)
    {
         
        switch (rating)
        {   
            case ReviewRating.Again:
                entry.BoxLevel = 1;
                break;
            case ReviewRating.Easy:
                entry. BoxLevel += 2;
                break;
            case ReviewRating.Good:
                entry.BoxLevel += 1;
                break;
            case ReviewRating.Hard:
                break;
            
        }
        
        if (entry.BoxLevel > 5) entry.BoxLevel = 5;

        
        entry.NextReviewDate =  DateTime.Now.AddDays(entry.BoxLevel);
    }
}