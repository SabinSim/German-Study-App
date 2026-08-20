using Avalonia.Controls;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class StatsView : UserControl
{
    public StatsView()
    {
        InitializeComponent();

        Loaded += async (_, _) =>
        {
            if (DataContext is StatsViewModel vm)
            {
                await vm.LoadCommand.ExecuteAsync(null);
            }
        };
    }
}
