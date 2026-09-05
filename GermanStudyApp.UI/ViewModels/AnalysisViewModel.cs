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
    private readonly IDeckRepository _deckRepository;

    [ObservableProperty]
    private string _germanText = string.Empty;

    // 단어를 저장할 때 어떤 덱에 넣을지 고르는 드롭다운용 목록/선택값.
    public ObservableCollection<Deck> AvailableDecks { get; } = new();

    [ObservableProperty]
    private Deck? _selectedDeck;

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
        _analysisService = AnalysisServiceFactory.Create();
        _vocabRepository = new VocabRepository(AnalysisServiceFactory.Create());
        _deckRepository = new DeckRepository();
    }

    [RelayCommand]
    private async Task LoadDecksAsync()
    {
        var decks = await _deckRepository.GetAllAsync();

        AvailableDecks.Clear();
        foreach (var deck in decks)
        {
            AvailableDecks.Add(deck);
        }

        SelectedDeck ??= AvailableDecks.FirstOrDefault();
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
        catch (HttpRequestException ex) when (ex.Message.Contains("401"))
        {
            ErrorMessage = "API authentication failed. Please set your OPENAI_API_KEY environment variable with a valid API key.";
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

        if (SelectedDeck is null)
        {
            StatusMessage = "Please choose a deck to save into.";
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
                    DeckId = SelectedDeck.Id,
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
