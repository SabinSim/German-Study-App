using GermanStudyApp.Core.Models;
using GermanStudyApp.Infrastructure;

namespace GermanStudyApp.Tests;

public class LeitnerFlashcardServiceTests
{
    [Fact]
    public void ApplyReviewResult_WhenAgain_ResetsBoxLevelToOne()
    {
        var entry = new VocabEntry
        {
            Word = "Haus",
            Meaning = "house",
            DeckId = 1,
            BoxLevel = 3
        };
        
        var service = new LeitnerFlashcardService();
        
        service.ApplyReviewResult(entry, ReviewRating.Again);
        
        Assert.Equal(1, entry.BoxLevel);
    }
    
    [Fact]
    public void ApplyReviewResult_WhenHard_KeepsBoxLevelUnchanged()
    {
        var entry = new VocabEntry
        {
            Word = "Haus",
            Meaning = "house",
            DeckId = 1,
            BoxLevel = 3
        };
        
        var service = new LeitnerFlashcardService();
        
        service.ApplyReviewResult(entry, ReviewRating.Hard);
        
        Assert.Equal(3, entry.BoxLevel);
    }
    
    [Fact]
    public void ApplyReviewResult_WhenGood_IncreasesBoxLevelByOne()
    {
        var entry = new VocabEntry
        {
            Word = "Haus",
            Meaning = "house",
            DeckId = 1,
            BoxLevel = 2
        };
        
        var service = new LeitnerFlashcardService();
        
        service.ApplyReviewResult(entry, ReviewRating.Good);
        
        Assert.Equal(3, entry.BoxLevel);
    }

    [Fact]
    public void ApplyReviewResult_WhenEasy_IncreasesBoxLevelByTwo()
    {
        var entry = new VocabEntry()
        {
            Word = "Haus",
            Meaning = "house",
            DeckId = 1,
            BoxLevel = 2
        };
        
        var service = new LeitnerFlashcardService();
        
        service.ApplyReviewResult(entry, ReviewRating.Easy);
        
        Assert.Equal(4, entry.BoxLevel);
    }
    
    [Fact]
    public void ApplyReviewResult_WhenBoxLevelIsAtMaxLevel_DoesNotExceedMaxLevel()
    {
        var entry = new VocabEntry()
        {
            Word = "Haus",
            Meaning = "house",
            DeckId = 1,
            BoxLevel = 5 // Assuming 5 is the max level
        };
        
        var service = new LeitnerFlashcardService();
        
        service.ApplyReviewResult(entry, ReviewRating.Good);
        
        Assert.Equal(5, entry.BoxLevel); // Should not exceed max level
    }

    [Fact]
    public void ApplyReviewResult_WhenEasyAtMaxLevel_DoesNotExceedMaxLevel()
    {
        var entry = new VocabEntry()
        {
            Word = "Haus",
            Meaning = "house",
            DeckId = 1,
            BoxLevel = 5
        };
    
        var service = new LeitnerFlashcardService();
    
        service.ApplyReviewResult(entry, ReviewRating.Easy);
    
        Assert.Equal(5, entry.BoxLevel);
    }
}
