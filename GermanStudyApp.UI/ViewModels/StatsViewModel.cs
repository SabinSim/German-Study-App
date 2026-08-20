using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Infrastructure;

namespace GermanStudyApp.UI.ViewModels;

public partial class StatsViewModel : ObservableObject
{
    private const int LeechThreshold = 5;

    private readonly IVocabRepository _vocabRepository;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private int _totalWords;

    [ObservableProperty]
    private int _totalLeeches;

    public ObservableCollection<DailyStat> DailyStats { get; } = new();

    public ObservableCollection<BoxLevelStat> BoxLevelStats { get; } = new();

    public StatsViewModel()
    {
        _vocabRepository = new VocabRepository();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var all = await _vocabRepository.GetAllAsync();

            TotalWords = all.Count;
            TotalLeeches = all.Count(e => e.AgainCount >= LeechThreshold);

            DailyStats.Clear();
            var byDate = all
                .GroupBy(e => e.DateAdded.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new DailyStat(g.Key, g.Count()));

            foreach (var stat in byDate)
            {
                DailyStats.Add(stat);
            }

            BoxLevelStats.Clear();
            for (var box = 1; box <= 5; box++)
            {
                var count = all.Count(e => e.BoxLevel == box);
                BoxLevelStats.Add(new BoxLevelStat(box, count));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred while loading stats: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
