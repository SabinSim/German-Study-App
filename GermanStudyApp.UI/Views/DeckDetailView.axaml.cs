using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using GermanStudyApp.Core.Models;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class DeckDetailView : UserControl
{
    // 뒤로가기 버튼을 눌렀을 때, 부모(덱 목록 화면)에게 "돌아가고 싶다"고 알려주는 이벤트.
    public event EventHandler? BackRequested;

    public DeckDetailView()
    {
        InitializeComponent();

        Loaded += async (_, _) =>
        {
            if (DataContext is DeckDetailViewModel vm)
            {
                await vm.LoadCommand.ExecuteAsync(null);
            }
        };
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    private async void OnChooseFileClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DeckDetailViewModel vm)
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

    private void OnEditWordClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is VocabEntry entry && DataContext is DeckDetailViewModel vm)
        {
            vm.StartEditWordCommand.Execute(entry);
        }
    }

    private async void OnDeleteWordClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is VocabEntry entry)
        {
            var dialog = new ConfirmationDialog
            {
                Message = $"Are you sure you want to delete '{entry.Word}'?"
            };

            if (TopLevel.GetTopLevel(this) is Window owner)
            {
                await dialog.ShowDialog(owner);

                if (dialog.Result && DataContext is DeckDetailViewModel vm)
                {
                    await vm.DeleteWordAsync(entry);
                }
            }
        }
    }

    private async void OnToggleSuspendClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is VocabEntry entry && DataContext is DeckDetailViewModel vm)
        {
            await vm.ToggleSuspendAsync(entry);
        }
    }
}
