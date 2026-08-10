using Avalonia.Controls;
using Avalonia.Interactivity;
using GermanStudyApp.Core.Models;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class VocabNotebookView : UserControl
{
    public VocabNotebookView()
    {
        InitializeComponent();

        // 이 화면(탭)이 처음 화면에 나타날 때, 자동으로 단어장을 한 번 불러온다.
        Loaded += async (_, _) =>
        {
            if (DataContext is VocabNotebookViewModel vm)
            {
                await vm.LoadCommand.ExecuteAsync(null);
            }
        };
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

                if (dialog.Result && DataContext is VocabNotebookViewModel vm)
                {
                    await vm.DeleteWordAsync(entry);
                }
            }
        }
    }

    private async void OnDeleteAllClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new ConfirmationDialog
        {
            Message = "Are you sure you want to delete ALL vocabulary words? This action cannot be undone!"
        };

        if (TopLevel.GetTopLevel(this) is Window owner)
        {
            await dialog.ShowDialog(owner);

            if (dialog.Result && DataContext is VocabNotebookViewModel vm)
            {
                await vm.DeleteAllAsync();
            }
        }
    }
}
