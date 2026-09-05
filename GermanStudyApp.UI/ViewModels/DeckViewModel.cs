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

public partial class DeckViewModel : ObservableObject
{
    private readonly IDeckRepository _deckRepository;

    // 덱 카드를 클릭했을 때, 부모(MainWindow)에게 "이 덱을 열어줘"라고 알려주는 이벤트.
    public event EventHandler<Deck>? DeckOpened;

    [ObservableProperty]
    private string _newDeckName = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _statusMessage;

    // null이면 "새 덱 만들기" 모드, 값이 있으면 "그 덱을 수정하는 중" 모드.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEditing))]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private Deck? _editingDeck;

    public bool IsEditing => EditingDeck is not null;
    public string SaveButtonText => IsEditing ? "Save Changes" : "Create Deck";

    // 순환 참조 방지 계산(GetDescendantIds)에 쓰는, 덱 전체의 납작한(flat) 목록.
    public ObservableCollection<Deck> AllDecks { get; } = new();

    // 화면에 계층 구조로 보여줄, 들여쓰기가 적용된 목록.
    public ObservableCollection<DeckDisplayItem> DisplayDecks { get; } = new();

    public DeckViewModel()
    {
        _deckRepository = new DeckRepository();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;

        try
        {
            var decks = await _deckRepository.GetAllAsync();

            AllDecks.Clear();
            foreach (var deck in decks)
            {
                AllDecks.Add(deck);
            }

            RebuildDisplayList(decks);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load decks: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveDeckAsync()
    {
        if (string.IsNullOrWhiteSpace(NewDeckName))
        {
            StatusMessage = "Please enter a deck name.";
            return;
        }

        IsBusy = true;

        try
        {
            if (EditingDeck is not null)
            {
                // 이름만 바꾼다. 부모를 바꾸고 싶으면 목록에서 드래그해서 옮긴다.
                EditingDeck.Name = NewDeckName.Trim();
                await _deckRepository.UpdateAsync(EditingDeck);
            }
            else
            {
                // 새 덱은 항상 최상위로 만들어진다. 다른 덱 밑에 넣고 싶으면
                // 만든 뒤 목록에서 그 덱 위로 드래그해서 옮기면 된다.
                var deck = new Deck
                {
                    Name = NewDeckName.Trim(),
                    ParentDeckId = null,
                };

                await _deckRepository.SaveAsync(deck);
            }

            NewDeckName = string.Empty;
            EditingDeck = null;
            StatusMessage = null;

            await LoadAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to save deck: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenDeck(Deck deck)
    {
        DeckOpened?.Invoke(this, deck);
    }

    [RelayCommand]
    private void StartEdit(Deck deck)
    {
        EditingDeck = deck;
        NewDeckName = deck.Name;
        StatusMessage = null;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        EditingDeck = null;
        NewDeckName = string.Empty;
        StatusMessage = null;
    }

    // 드래그 앤 드롭으로 덱을 다른 덱 위에 놓았을 때 호출된다.
    // draggedDeck을 targetDeck의 자식으로 옮긴다.
    public async Task<bool> MoveDeckAsync(Deck draggedDeck, Deck targetDeck)
    {
        StatusMessage = null;

        if (draggedDeck.Id == targetDeck.Id)
        {
            // 자기 자신 위에 놓은 경우 - 아무 일도 하지 않는다.
            return false;
        }

        // targetDeck이 draggedDeck 자신의 자손이면, 원형 구조가 생기므로 막는다.
        var descendantIds = GetDescendantIds(draggedDeck.Id, AllDecks.ToList());
        if (descendantIds.Contains(targetDeck.Id))
        {
            StatusMessage = "Can't move a deck into one of its own sub-decks.";
            return false;
        }

        try
        {
            draggedDeck.ParentDeckId = targetDeck.Id;
            await _deckRepository.UpdateAsync(draggedDeck);
            await LoadAsync();
            return true;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to move deck: {ex.Message}";
            return false;
        }
    }

    public async Task DeleteDeckAsync(Deck deck)
    {
        StatusMessage = null;

        try
        {
            await _deckRepository.DeleteAsync(deck);

            if (EditingDeck?.Id == deck.Id)
            {
                CancelEdit();
            }

            await LoadAsync();
        }
        catch (Exception)
        {
            StatusMessage = $"Couldn't delete '{deck.Name}'. Make sure it has no words or sub-decks inside it first.";
        }
    }

    private void RebuildDisplayList(List<Deck> decks)
    {
        DisplayDecks.Clear();

        var rootDecks = decks.Where(d => d.ParentDeckId is null).OrderBy(d => d.Name);

        foreach (var root in rootDecks)
        {
            AddWithChildren(root, decks, 0);
        }
    }

    private void AddWithChildren(Deck deck, List<Deck> allDecks, int indentLevel)
    {
        DisplayDecks.Add(new DeckDisplayItem(deck, indentLevel));

        var children = allDecks.Where(d => d.ParentDeckId == deck.Id).OrderBy(d => d.Name);

        foreach (var child in children)
        {
            AddWithChildren(child, allDecks, indentLevel + 1);
        }
    }

    private static HashSet<int> GetDescendantIds(int deckId, List<Deck> allDecks)
    {
        var result = new HashSet<int>();
        var children = allDecks.Where(d => d.ParentDeckId == deckId);

        foreach (var child in children)
        {
            result.Add(child.Id);

            foreach (var id in GetDescendantIds(child.Id, allDecks))
            {
                result.Add(id);
            }
        }

        return result;
    }
}
