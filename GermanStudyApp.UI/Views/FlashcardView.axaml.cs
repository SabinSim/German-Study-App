using Avalonia.Controls;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class FlashcardView : UserControl
{
    public FlashcardView()
    {
        InitializeComponent();

        Loaded += async (_, _) =>
        {
            if (DataContext is FlashcardViewModel vm)
            {
                await vm.LoadDecksCommand.ExecuteAsync(null);
            }
        };
    }
}
