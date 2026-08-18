using GermanStudyApp.Infrastructure;
using GermanStudyApp.Infrastructure.Data;
using GermanStudyApp.Core.Models;

// 데이터베이스 초기화
DbInitializer.Initialize();

Console.WriteLine("=== Testing Deck Repository ===");
var deckRepository = new DeckRepository();
var decks = await deckRepository.GetAllAsync();

Console.WriteLine($"Found {decks.Count} deck(s):");
foreach (var deck in decks)
{
    Console.WriteLine($"  - ID: {deck.Id}, Name: {deck.Name}");
}
Console.WriteLine("Deck test passed!\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Error: OPENAI_API_KEY environment variable is not set.");
    Console.WriteLine("Please set your OpenAI API key using:");
    Console.WriteLine("  export OPENAI_API_KEY='your-api-key-here'");
    Console.WriteLine("\nBut the Deck Repository test was successful!");
    return;
}

var httpClient = new HttpClient();

var service = new OpenAiAnalysisService(httpClient, apiKey);

var result = await service.AnalyzeAsync("Ich gehe heute in die Schule.", TargetLanguage.English);

Console.WriteLine(result.Translation);
Console.WriteLine(result.LiteralTranslation);
foreach (var wordAnalysis in result.WordAnalyses)
{
    Console.WriteLine($"Word: {wordAnalysis.Word}");
    Console.WriteLine($"Meaning: {wordAnalysis.Meaning}");
    Console.WriteLine($"Gender: {wordAnalysis.Gender}");
    Console.WriteLine($"OriginalWord: {wordAnalysis.OriginalWord}");
    Console.WriteLine($"PastParticiple: {wordAnalysis.PastParticiple}");
    Console.WriteLine($"GrammarExplanation: {wordAnalysis.GrammarExplanation}");
}

var repository = new VocabRepository();

var entry = new VocabEntry
{
    Word = "Schule",
    Meaning = "학교",
    DateAdded = DateTime.Now,
    NextReviewDate = DateTime.Now,
    BoxLevel = 1,
    DeckId = 1
};

await repository.SaveAsync(entry);

var allWords = await repository.GetAllAsync();
foreach (var word in allWords)
{
    Console.WriteLine($"{word.Word} -  {word.Meaning}");
}