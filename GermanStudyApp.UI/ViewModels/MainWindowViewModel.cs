using CommunityToolkit.Mvvm.ComponentModel;
using GermanStudyApp.Core.Models;

namespace GermanStudyApp.UI.ViewModels;

// 화면 전체를 총괄하는 ViewModel. 탭들이 각자 쓸 ViewModel을 여기서 하나씩 만들어서 들고 있는다.
// Decks 탭 안에서는 "덱 목록"과 "덱 상세(단어 목록/추가/임포트)" 두 화면을 전환한다.
public partial class MainWindowViewModel : ObservableObject
{
    public DeckViewModel DeckVM { get; } = new();
    public FlashcardViewModel FlashcardVM { get; } = new();
    public StatsViewModel StatsVM { get; } = new();

    public DeckDetailViewModel DeckDetailVM { get; } = new();

    // false면 덱 목록, true면 특정 덱 안(상세 화면)을 보고 있는 상태.
    [ObservableProperty]
    private bool _isViewingDeckDetail;

    public MainWindowViewModel()
    {
        DeckVM.DeckOpened += (_, deck) => OpenDeck(deck);
    }

    private void OpenDeck(Deck deck)
    {
        DeckDetailVM.SetDeck(deck);
        IsViewingDeckDetail = true;
    }

    public void CloseDeckDetail()
    {
        IsViewingDeckDetail = false;
    }
}
