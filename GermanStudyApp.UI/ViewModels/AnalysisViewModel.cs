using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;
using GermanStudyApp.Infrastructure;

namespace GermanStudyApp.UI.ViewModels;

public partial class AnalysisViewModel : ObservableObject
{
    private readonly IGermanAnalysisService _analysisService;
    private readonly IVocabRepository _vocabRepository;

    [ObservableProperty]
    private string _germanText = string.Empty;

    [ObservableProperty]
    private TargetLanguage _selectedTargetLanguage = TargetLanguage.English;

    public TargetLanguage[] TargetLanguages { get; } = { TargetLanguage.English, TargetLanguage.Korean };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayedTranslation))]
    private bool _showLiteralTranslation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayedTranslation))]
    [NotifyPropertyChangedFor(nameof(HasResult))]
    private AnalyzedSentence? _result;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _statusMessage;

    public bool HasResult => Result is not null;

    public ObservableCollection<WordAnalysisItem> WordItems { get; } = new();

    public string DisplayedTranslation =>
        Result is null
            ? string.Empty
            : ShowLiteralTranslation ? Result.LiteralTranslation : Result.Translation;

    public AnalysisViewModel()
    {
        // 콘솔 테스트 때와 똑같이, 환경 변수에서 API 키를 읽어온다.
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
        _analysisService = new OpenAiAnalysisService(new HttpClient(), apiKey);
        _vocabRepository = new VocabRepository();
    }

    [RelayCommand]
    private async Task AnalyzeAsync()
    {
        if (string.IsNullOrWhiteSpace(GermanText))
        {
            ErrorMessage = "Please enter a German sentence to analyze.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var analyzed = await _analysisService.AnalyzeAsync(GermanText, SelectedTargetLanguage);
            Result = analyzed;

            WordItems.Clear();
            foreach (var word in analyzed.WordAnalyses)
            {
                WordItems.Add(new WordAnalysisItem(word));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred while analyzing: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveSelectedWordsAsync()
    {
        var selected = WordItems.Where(item => item.IsSelected).ToList();

        if (selected.Count == 0)
        {
            StatusMessage = "Please check at least one word to save.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            foreach (var item in selected)
            {
                var entry = new VocabEntry
                {
                    Word = item.Word,
                    Meaning = item.Meaning,
                    DateAdded = DateTime.Now,
                    NextReviewDate = DateTime.Now,
                    BoxLevel = 1,
                };

                await _vocabRepository.SaveAsync(entry);
                item.IsSelected = false;
            }

            StatusMessage = $"Saved {selected.Count} word(s) to your vocabulary.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred while saving: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
