using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GermanStudyApp.UI.Converters;

// 버튼에 지금 상태에 맞는 글자를 보여준다: 일시정지 중이면 "Resume", 아니면 "Suspend".
public class SuspendButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuspended)
        {
            return isSuspended ? "Resume" : "Suspend";
        }

        return "Suspend";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
