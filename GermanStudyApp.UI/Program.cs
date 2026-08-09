using System;
using Avalonia;
using GermanStudyApp.Core.Models;

namespace GermanStudyApp.UI;

class Program
{
    // 앱의 진짜 시작점. Avalonia는 Main에서 직접 Window를 띄우지 않고
    // AppBuilder를 통해 App.axaml.cs로 제어권을 넘긴다.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

