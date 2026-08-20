using CommunityToolkit.Mvvm.ComponentModel;

namespace GermanStudyApp.UI.ViewModels;

// 화면 전체를 총괄하는 ViewModel. 탭 다섯 개(덱 화면, 분석 화면, 단어장 화면, 플래시카드 화면, 통계 화면)가
// 각자 쓸 ViewModel을 여기서 하나씩 만들어서 들고 있는다.
public partial class MainWindowViewModel : ObservableObject
{
    public DeckViewModel DeckVM { get; } = new();
    public AnalysisViewModel AnalysisVM { get; } = new();
    public VocabNotebookViewModel VocabVM { get; } = new();
    public FlashcardViewModel FlashcardVM { get; } = new();
    public StatsViewModel StatsVM { get; } = new();
}
