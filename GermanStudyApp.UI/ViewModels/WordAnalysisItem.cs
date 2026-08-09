using CommunityToolkit.Mvvm.ComponentModel;
using GermanStudyApp.Core.Models;

namespace GermanStudyApp.UI.ViewModels;

// WordAnalysis 자체에는 "체크박스 선택 여부"라는 개념이 없다 (Core는 UI를 몰라야 하니까).
// 그래서 화면에서만 쓸 "선택 여부"를 덧붙인 래퍼(포장) 클래스를 UI 쪽에 따로 만든다.
public partial class WordAnalysisItem : ObservableObject
{
    public WordAnalysis Source { get; }

    [ObservableProperty]
    private bool _isSelected;

    public string Word => Source.Word;
    public string Meaning => Source.Meaning;
    public string Gender => Source.Gender;
    public string OriginalWord => Source.OriginalWord;
    public string PastParticiple => Source.PastParticiple;
    public string GrammarExplanation => Source.GrammarExplanation;

    public WordAnalysisItem(WordAnalysis source)
    {
        Source = source;
    }
}
