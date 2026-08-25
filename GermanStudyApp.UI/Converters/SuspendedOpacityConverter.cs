using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GermanStudyApp.UI.Converters;

// 일시정지된 단어는 화면에서 살짝 흐리게 보이도록 투명도를 낮춘다.
public class SuspendedOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuspended)
        {
            return isSuspended ? 0.5 : 1.0;
        }

        return 1.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
