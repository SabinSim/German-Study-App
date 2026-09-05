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

public partial class VocabNotebookViewModel : ObservableObject
{
    private readonly IVocabRepository _vocabRepository;
    private readonly IDeckRepository _deckRepository;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _successMessage;

    // 필터 드롭다운용 덱 목록. 선택된 게 없으면(null) 전체를 보여준다.
    public ObservableCollection<Deck> AvailableDecks { get; } = new();

    [ObservableProperty]
    private Deck? _selectedDeckFilter;

    // 단어 이름/뜻으로 검색하는 검색창용 텍스트.
    [ObservableProperty]
    private string _searchText = string.Empty;

    public ObservableCollection<VocabDateGroup> GroupedEntries { get; } = new();

    public VocabNotebookViewModel()
    {
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

    // 필터 드롭다운에서 다른 덱을 고르면, 목록을 자동으로 다시 불러온다.
    partial void OnSelectedDeckFilterChanged(Deck? value)
    {
        _ = LoadAsync();
    }

    // 검색창에 글자를 입력할 때마다, 목록을 자동으로 다시 불러온다.
    partial void OnSearchTextChanged(string value)
    {
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            var all = await _vocabRepository.GetAllAsync();

            var filtered = SelectedDeckFilter is null
                ? all
                : all.Where(e => e.DeckId == SelectedDeckFilter.Id).ToList();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered
                    .Where(e => e.Word.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                || e.Meaning.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            GroupedEntries.Clear();

            // DateAdded의 "날짜" 부분(시간은 무시)으로 묶고,
            // 최근 날짜가 위로 오도록 내림차순 정렬한다.
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
            ErrorMessage = $"An error occurred while loading your vocabulary: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
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

    public async Task DeleteAllAsync()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            await _vocabRepository.DeleteAllAsync();
            SuccessMessage = "All words deleted successfully.";
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred while deleting all words: {ex.Message}";
        }
    }
}
