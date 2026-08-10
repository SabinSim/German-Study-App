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

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;
    
    [ObservableProperty]
    private string? _successMessage;

    public ObservableCollection<VocabDateGroup> GroupedEntries { get; } = new();

    public VocabNotebookViewModel()
    {
        _vocabRepository = new VocabRepository();
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

            GroupedEntries.Clear();

            // DateAdded의 "날짜" 부분(시간은 무시)으로 묶고,
            // 최근 날짜가 위로 오도록 내림차순 정렬한다.
            var groups = all
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
