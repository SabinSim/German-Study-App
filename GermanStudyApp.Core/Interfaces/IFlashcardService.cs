using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Core.Interfaces;

public interface IFlashcardService
{
    void ApplyReviewResult(VocabEntry entry, bool wasCorrect);
}

