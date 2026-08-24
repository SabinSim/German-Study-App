using CommunityToolkit.Mvvm.ComponentModel;
using GermanStudyApp.Core.Models;

namespace GermanStudyApp.UI.ViewModels;

// 미리보기 목록에서 각 단어 옆에 체크박스를 달기 위한 래퍼.
// (분석 화면의 WordAnalysisItem이랑 같은 패턴.)
public partial class ImportEntryItem : ObservableObject
{
    public VocabEntry Entry { get; }

    public string Word => Entry.Word;
    public string Meaning => Entry.Meaning;

    [ObservableProperty]
    private bool _isSelected = true;

    public ImportEntryItem(VocabEntry entry)
    {
        Entry = entry;
    }
}
