using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;
using GermanStudyApp.Infrastructure;

namespace GermanStudyApp.UI.ViewModels;

public partial class FlashcardViewModel : ObservableObject
{
    private readonly IVocabRepository _vocabRepository;
    private readonly IDeckRepository _deckRepository;
    private readonly IFlashcardService _flashcardService;

    // 오늘 복습해야 할 단어들이 순서대로 대기하는 줄(큐).
    private Queue<VocabEntry> _dueQueue = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCurrentCard))]
    private VocabEntry? _currentCard;

    [ObservableProperty]
    private bool _isFlipped;

    [ObservableProperty]
    private int _remainingCount;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _statusMessage;

    // 복습할 덱을 고르는 드롭다운용. 선택된 게 없으면(null) 전체 덱에서 복습한다.
    public ObservableCollection<Deck> AvailableDecks { get; } = new();

    [ObservableProperty]
    private Deck? _selectedDeckFilter;

    public bool HasCurrentCard => CurrentCard is not null;

    public FlashcardViewModel()
    {
        _vocabRepository = new VocabRepository();
        _deckRepository = new DeckRepository();
        _flashcardService = new LeitnerFlashcardService();
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

    [RelayCommand]
    private async Task StartSessionAsync()
    {
        IsBusy = true;
        StatusMessage = null;

        try
        {
            var all = await _vocabRepository.GetAllAsync();

            if (SelectedDeckFilter is not null)
            {
                all = all.Where(e => e.DeckId == SelectedDeckFilter.Id).ToList();
            }

            // 오늘 날짜 기준으로 "복습해야 할 때가 된" 단어만 골라서 큐에 담는다.
            var due = all.Where(e => e.NextReviewDate <= DateTime.Now).ToList();

            _dueQueue = new Queue<VocabEntry>(due);
            RemainingCount = _dueQueue.Count;

            if (_dueQueue.Count == 0)
            {
                CurrentCard = null;
                StatusMessage = "No words are due for review right now.";
            }
            else
            {
                IsFlipped = false;
                CurrentCard = _dueQueue.Dequeue();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"An error occurred while loading the review session: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void FlipCard()
    {
        IsFlipped = !IsFlipped;
    }

    [RelayCommand]
    private Task MarkAgainAsync() => AnswerAsync(ReviewRating.Again);

    [RelayCommand]
    private Task MarkHardAsync() => AnswerAsync(ReviewRating.Hard);

    [RelayCommand]
    private Task MarkGoodAsync() => AnswerAsync(ReviewRating.Good);

    [RelayCommand]
    private Task MarkEasyAsync() => AnswerAsync(ReviewRating.Easy);

    private async Task AnswerAsync(ReviewRating rating)
    {
        if (CurrentCard is null)
        {
            return;
        }

        IsBusy = true;

        try
        {
            // Again/Hard는 "아직 잘 모른다"는 뜻이니까, 같은 세션 안에서 다시 볼 수 있도록
            // 큐 맨 뒤에 다시 넣어둔다 (재도전 기회).
            var shouldRequeue = rating is ReviewRating.Again or ReviewRating.Hard;

            // 1) 메모리에서 박스 레벨 / 다음 복습 날짜를 계산하고
            _flashcardService.ApplyReviewResult(CurrentCard, rating);
            // 2) 그 결과를 데이터베이스에 실제로 반영한다
            await _vocabRepository.UpdateAsync(CurrentCard);

            if (shouldRequeue)
            {
                _dueQueue.Enqueue(CurrentCard);
            }

            RemainingCount = _dueQueue.Count;
            IsFlipped = false;

            CurrentCard = _dueQueue.Count > 0 ? _dueQueue.Dequeue() : null;

            if (CurrentCard is null)
            {
                StatusMessage = "You've reviewed all the words due today. Well done!";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"An error occurred while saving your answer: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
