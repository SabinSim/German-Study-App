using System;
using System.Collections.Generic;
using GermanStudyApp.Core.Models;

namespace GermanStudyApp.UI.ViewModels;

// 단어장 화면에서 "날짜별로 묶은 한 덩어리"를 표현하는 타입.
// 예: 2026-08-09 라는 Date에, 그날 저장한 단어들(Entries)이 들어있다.
public class VocabDateGroup
{
    public DateTime Date { get; }
    public IReadOnlyList<VocabEntry> Entries { get; }

    public VocabDateGroup(DateTime date, IReadOnlyList<VocabEntry> entries)
    {
        Date = date;
        Entries = entries;
    }
}
