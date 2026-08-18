using GermanStudyApp.Core.Models;

namespace GermanStudyApp.UI.ViewModels;

// 화면에 덱을 계층 구조처럼 보여주기 위해, Deck에 "들여쓰기 단계"를 덧붙인 래퍼.
public class DeckDisplayItem
{
    public Deck Source { get; }
    public int IndentLevel { get; }

    public DeckDisplayItem(Deck source, int indentLevel)
    {
        Source = source;
        IndentLevel = indentLevel;
    }

    public int Id => Source.Id;
    public string Name => Source.Name;

    public string IndentedName =>
        (IndentLevel > 0 ? new string(' ', IndentLevel * 4) + "└ " : string.Empty) + Name;
}
