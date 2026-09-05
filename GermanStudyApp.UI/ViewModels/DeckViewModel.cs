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
    private Deck? _selectedParentDeck;

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

    // 부모 덱 선택용 드롭다운에 쓸, 덱 전체의 납작한(flat) 목록.
    public ObservableCollection<Deck> AllDecks { get; } = new();

    // 부모 선택 드롭다운에 실제로 보여줄 목록 (수정 중인 덱 자신과 그 하위 덱은 제외 - 순환 방지).
    public ObservableCollection<Deck> ParentOptions { get; } = new();

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
            RefreshParentOptions();
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
                EditingDeck.Name = NewDeckName.Trim();
                EditingDeck.ParentDeckId = SelectedParentDeck?.Id;
                await _deckRepository.UpdateAsync(EditingDeck);
            }
            else
            {
                var deck = new Deck
                {
                    Name = NewDeckName.Trim(),
                    ParentDeckId = SelectedParentDeck?.Id,
                };

                await _deckRepository.SaveAsync(deck);
            }

            NewDeckName = string.Empty;
            SelectedParentDeck = null;
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
        RefreshParentOptions();
        SelectedParentDeck = ParentOptions.FirstOrDefault(d => d.Id == deck.ParentDeckId);
    }

    [RelayCommand]
    private void CancelEdit()
    {
        EditingDeck = null;
        NewDeckName = string.Empty;
        SelectedParentDeck = null;
        StatusMessage = null;
        RefreshParentOptions();
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

    // 부모로 고를 수 있는 덱 목록을 다시 계산한다. 수정 중인 덱 자신과, 그 밑에 있는
    // 모든 하위 덱은 목록에서 뺀다 (자기 자신이나 자기 자손을 부모로 고르면 원형 구조가 생기니까).
    private void RefreshParentOptions()
    {
        ParentOptions.Clear();

        var excludedIds = EditingDeck is null
            ? new HashSet<int>()
            : GetDescendantIds(EditingDeck.Id, AllDecks.ToList());

        if (EditingDeck is not null)
        {
            excludedIds.Add(EditingDeck.Id);
        }

        foreach (var deck in AllDecks.Where(d => !excludedIds.Contains(d.Id)))
        {
            ParentOptions.Add(deck);
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
