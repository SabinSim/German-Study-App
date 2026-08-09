using System;
using System.Collections.Generic;
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

    public bool HasCurrentCard => CurrentCard is not null;

    public FlashcardViewModel()
    {
        _vocabRepository = new VocabRepository();
        _flashcardService = new LeitnerFlashcardService();
    }

    [RelayCommand]
    private async Task StartSessionAsync()
    {
        IsBusy = true;
        StatusMessage = null;

        try
        {
            var all = await _vocabRepository.GetAllAsync();

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
    private Task MarkCorrectAsync() => AnswerAsync(wasCorrect: true);

    [RelayCommand]
    private Task MarkWrongAsync() => AnswerAsync(wasCorrect: false);

    private async Task AnswerAsync(bool wasCorrect)
    {
        if (CurrentCard is null)
        {
            return;
        }

        IsBusy = true;

        try
        {
            // 1) 메모리에서 박스 레벨 / 다음 복습 날짜를 계산하고
            _flashcardService.ApplyReviewResult(CurrentCard, wasCorrect);
            // 2) 그 결과를 데이터베이스에 실제로 반영한다
            await _vocabRepository.UpdateAsync(CurrentCard);

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
