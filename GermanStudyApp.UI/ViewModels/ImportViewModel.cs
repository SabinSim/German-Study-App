using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;
using GermanStudyApp.Infrastructure;

namespace GermanStudyApp.UI.ViewModels;

public partial class ImportViewModel : ObservableObject
{
    private readonly IVocabImportService _importService;
    private readonly IVocabRepository _vocabRepository;
    private readonly IDeckRepository _deckRepository;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _statusMessage;

    public ObservableCollection<Deck> AvailableDecks { get; } = new();

    [ObservableProperty]
    private Deck? _selectedDeck;

    public ObservableCollection<ImportEntryItem> PreviewEntries { get; } = new();

    public bool HasPreview => PreviewEntries.Count > 0;

    public ImportViewModel()
    {
        _importService = new TxtVocabImportService();
        _vocabRepository = new VocabRepository(AnalysisServiceFactory.Create());
        _deckRepository = new DeckRepository();
    }

    [RelayCommand]
    private async Task LoadDecksAsync()
    {
        var decks = await _deckRepository.GetAllAsync();

        AvailableDecks.Clear();
        foreach (var deck in decks.OrderBy(d => d.Name))
        {
            AvailableDecks.Add(deck);
        }
    }

    // 파일을 실제로 여는 건 코드 비하인드(View)에서 하고, 읽어온 텍스트만 여기로 넘겨받는다.
    public void LoadPreviewFromText(string fileContent)
    {
        StatusMessage = null;
        PreviewEntries.Clear();

        if (SelectedDeck is null)
        {
            StatusMessage = "Please choose a deck first.";
            return;
        }

        try
        {
            var parsed = _importService.ParseTxt(fileContent, SelectedDeck.Id);

            foreach (var entry in parsed)
            {
                PreviewEntries.Add(new ImportEntryItem(entry));
            }

            OnPropertyChanged(nameof(HasPreview));

            if (PreviewEntries.Count == 0)
            {
                StatusMessage = "No words found in that file.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Couldn't read that file. Make sure each line looks like 'Word,Meaning'. ({ex.Message})";
        }
    }

    [RelayCommand]
    private async Task SaveSelectedAsync()
    {
        var selected = PreviewEntries.Where(item => item.IsSelected).ToList();

        if (selected.Count == 0)
        {
            StatusMessage = "Please check at least one word to save.";
            return;
        }

        IsBusy = true;

        try
        {
            foreach (var item in selected)
            {
                await _vocabRepository.SaveAsync(item.Entry);
            }

            StatusMessage = $"Saved {selected.Count} word(s).";
            PreviewEntries.Clear();
            OnPropertyChanged(nameof(HasPreview));
        }
        catch (Exception ex)
        {
            StatusMessage = $"An error occurred while saving: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
