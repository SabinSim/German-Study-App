using Avalonia.Controls;
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
}
