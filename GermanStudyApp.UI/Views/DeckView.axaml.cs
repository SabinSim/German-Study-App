using Avalonia.Controls;
using Avalonia.Interactivity;
using GermanStudyApp.Core.Models;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class DeckView : UserControl
{
    public DeckView()
    {
        InitializeComponent();

        Loaded += async (_, _) =>
        {
            if (DataContext is DeckViewModel vm)
            {
                await vm.LoadCommand.ExecuteAsync(null);
            }
        };
    }

    private void OnEditDeckClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Deck deck && DataContext is DeckViewModel vm)
        {
            vm.StartEditCommand.Execute(deck);
        }
    }

    private async void OnDeleteDeckClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Deck deck)
        {
            var dialog = new ConfirmationDialog
            {
                Message = $"Are you sure you want to delete '{deck.Name}'?"
            };

            if (TopLevel.GetTopLevel(this) is Window owner)
            {
                await dialog.ShowDialog(owner);

                if (dialog.Result && DataContext is DeckViewModel vm)
                {
                    await vm.DeleteDeckAsync(deck);
                }
            }
        }
    }
}
