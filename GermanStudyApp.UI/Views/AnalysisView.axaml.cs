using Avalonia.Controls;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class AnalysisView : UserControl
{
    public AnalysisView()
    {
        InitializeComponent();

        Loaded += async (_, _) =>
        {
            if (DataContext is AnalysisViewModel vm)
            {
                await vm.LoadDecksCommand.ExecuteAsync(null);
            }
        };
    }
}
