namespace GermanStudyApp.UI.ViewModels;

// 통계 화면에서 "몇 번 박스에 단어가 몇 개 있는지"를 표현하는 타입.
public class BoxLevelStat
{
    public int BoxLevel { get; }
    public int Count { get; }

    public BoxLevelStat(int boxLevel, int count)
    {
        BoxLevel = boxLevel;
        Count = count;
    }
}
