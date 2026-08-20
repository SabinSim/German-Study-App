using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GermanStudyApp.UI.Converters;

// AgainCount(지금까지 "Again"을 누른 횟수)가 이 값 이상이면 "리치"로 본다.
public class LeechConverter : IValueConverter
{
    private const int LeechThreshold = 5;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int againCount)
        {
            return againCount >= LeechThreshold;
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
