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

// 덱 하나를 열었을 때 보여주는 화면의 ViewModel.
// 그 덱 안의 단어 목록, 단어 직접 추가, txt 파일 일괄 임포트를 한 곳에서 다룬다.
public partial class DeckDetailViewModel : ObservableObject
{
    private readonly IVocabRepository _vocabRepository;
    private readonly IVocabImportService _importService;

    [ObservableProperty]
    private Deck? _deck;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _successMessage;

    // 단어 이름/뜻으로 검색하는 검색창용 텍스트.
    [ObservableProperty]
    private string _searchText = string.Empty;

    public ObservableCollection<VocabDateGroup> GroupedEntries { get; } = new();

    // "단어 추가" 폼 입력값. EditingEntry가 null이 아니면, 새로 만드는 게 아니라
    // 그 단어를 고치는 중이라는 뜻이다 (Deck 화면의 EditingDeck 패턴과 동일).
    [ObservableProperty]
    private string _newWord = string.Empty;

    [ObservableProperty]
    private string _newMeaning = string.Empty;

    [ObservableProperty]
    private bool _isSavingNewWord;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEditingWord))]
    [NotifyPropertyChangedFor(nameof(WordFormButtonText))]
    private VocabEntry? _editingEntry;

    public bool IsEditingWord => EditingEntry is not null;
    public string WordFormButtonText => IsEditingWord ? "Save changes" : "Add word";

    // txt 임포트 미리보기 목록.
    public ObservableCollection<ImportEntryItem> PreviewEntries { get; } = new();

    public bool HasPreview => PreviewEntries.Count > 0;

    public DeckDetailViewModel()
    {
        _vocabRepository = new VocabRepository(AnalysisServiceFactory.Create());
        _importService = new TxtVocabImportService();
    }

    // 덱 목록 화면에서 어떤 덱을 열지 알려줄 때 호출한다.
    public void SetDeck(Deck deck)
    {
        Deck = deck;
        ErrorMessage = null;
        SuccessMessage = null;
        PreviewEntries.Clear();
        OnPropertyChanged(nameof(HasPreview));
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (Deck is null)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var all = await _vocabRepository.GetAllAsync();

            var filtered = all.Where(e => e.DeckId == Deck.Id).ToList();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered
                    .Where(e => e.Word.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                || e.Meaning.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            GroupedEntries.Clear();

            var groups = filtered
                .GroupBy(e => e.DateAdded.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new VocabDateGroup(
                    g.Key,
                    g.OrderByDescending(e => e.DateAdded).ToList()));

            foreach (var group in groups)
            {
                GroupedEntries.Add(group);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred while loading this deck's words: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddWordAsync()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        if (Deck is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(NewWord) || string.IsNullOrWhiteSpace(NewMeaning))
        {
            ErrorMessage = "Please enter both a word and a meaning.";
            return;
        }

        IsSavingNewWord = true;

        try
        {
            if (EditingEntry is not null)
            {
                // 수정 모드: 기존 단어의 Word/Meaning만 바꾸고, 예문/박스 레벨 등은 그대로 둔다.
                EditingEntry.Word = NewWord.Trim();
                EditingEntry.Meaning = NewMeaning.Trim();
                await _vocabRepository.UpdateAsync(EditingEntry);

                SuccessMessage = $"Updated '{EditingEntry.Word}'.";
                EditingEntry = null;
            }
            else
            {
                var entry = new VocabEntry
                {
                    Word = NewWord.Trim(),
                    Meaning = NewMeaning.Trim(),
                    DeckId = Deck.Id,
                    DateAdded = DateTime.Now,
                    NextReviewDate = DateTime.Now,
                    BoxLevel = 1,
                };

                // 예문 생성은 VocabRepository.SaveAsync 안에서 자동으로 처리된다.
                await _vocabRepository.SaveAsync(entry);

                SuccessMessage = $"Saved '{entry.Word}'.";
            }

            NewWord = string.Empty;
            NewMeaning = string.Empty;

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred while saving the word: {ex.Message}";
        }
        finally
        {
            IsSavingNewWord = false;
        }
    }

    [RelayCommand]
    private void StartEditWord(VocabEntry entry)
    {
        EditingEntry = entry;
        NewWord = entry.Word;
        NewMeaning = entry.Meaning;
        ErrorMessage = null;
        SuccessMessage = null;
    }

    [RelayCommand]
    private void CancelEditWord()
    {
        EditingEntry = null;
        NewWord = string.Empty;
        NewMeaning = string.Empty;
    }

    public async Task DeleteWordAsync(VocabEntry entry)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            await _vocabRepository.DeleteAsync(entry);
            SuccessMessage = "Word deleted successfully.";
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred while deleting the word: {ex.Message}";
        }
    }

    public async Task ToggleSuspendAsync(VocabEntry entry)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            entry.IsSuspended = !entry.IsSuspended;
            await _vocabRepository.UpdateAsync(entry);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred while updating the word: {ex.Message}";
        }
    }

    // 파일을 실제로 여는 건 코드 비하인드(View)에서 하고, 읽어온 텍스트만 여기로 넘겨받는다.
    public void LoadPreviewFromText(string fileContent)
    {
        if (Deck is null)
        {
            return;
        }

        ErrorMessage = null;
        PreviewEntries.Clear();

        try
        {
            var parsed = _importService.ParseTxt(fileContent, Deck.Id);

            foreach (var entry in parsed)
            {
                PreviewEntries.Add(new ImportEntryItem(entry));
            }

            OnPropertyChanged(nameof(HasPreview));

            if (PreviewEntries.Count == 0)
            {
                ErrorMessage = "No words found in that file.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Couldn't read that file. Make sure each line looks like 'Word,Meaning'. ({ex.Message})";
        }
    }

    [RelayCommand]
    private async Task SaveImportedAsync()
    {
        var selected = PreviewEntries.Where(item => item.IsSelected).ToList();

        if (selected.Count == 0)
        {
            ErrorMessage = "Please check at least one word to save.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            foreach (var item in selected)
            {
                await _vocabRepository.SaveAsync(item.Entry);
            }

            SuccessMessage = $"Saved {selected.Count} word(s).";
            PreviewEntries.Clear();
            OnPropertyChanged(nameof(HasPreview));
            await LoadAsync();
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
