using GermanStudyApp.Infrastructure;
using GermanStudyApp.Core.Models;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

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
    BoxLevel = 1
};

await repository.SaveAsync(entry);

var allWords = await repository.GetAllAsync();
foreach (var word in allWords)
{
    Console.WriteLine($"{word.Word} -  {word.Meaning}");
}