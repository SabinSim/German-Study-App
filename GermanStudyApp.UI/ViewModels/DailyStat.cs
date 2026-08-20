using System;

namespace GermanStudyApp.UI.ViewModels;

// 통계 화면에서 "이 날짜에 단어를 몇 개 저장했는지"를 표현하는 타입.
public class DailyStat
{
    public DateTime Date { get; }
    public int Count { get; }

    public DailyStat(DateTime date, int count)
    {
        Date = date;
        Count = count;
    }
}
