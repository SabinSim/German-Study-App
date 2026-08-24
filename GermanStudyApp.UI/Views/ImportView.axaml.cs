using System.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class ImportView : UserControl
{
    public ImportView()
    {
        InitializeComponent();

        Loaded += async (_, _) =>
        {
            if (DataContext is ImportViewModel vm)
            {
                await vm.LoadDecksCommand.ExecuteAsync(null);
            }
        };
    }

    private async void OnChooseFileClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ImportViewModel vm)
        {
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose a .txt file",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Text files") { Patterns = new[] { "*.txt" } }
            }
        });

        var file = files.FirstOrDefault();
        if (file is null)
        {
            return;
        }

        await using var stream = await file.OpenReadAsync();
        using var reader = new System.IO.StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        vm.LoadPreviewFromText(content);
    }
}
