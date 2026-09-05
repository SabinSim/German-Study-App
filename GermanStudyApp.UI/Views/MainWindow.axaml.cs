using Avalonia.Controls;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DeckDetailViewControl.BackRequested += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.CloseDeckDetail();
            }
        };
    }
}
