using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GermanStudyApp.UI.Views;

public partial class ConfirmationDialog : Window
{
    public bool Result { get; private set; }
    
    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
    
    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<ConfirmationDialog, string>(nameof(Message), "Are you sure?");

    public ConfirmationDialog()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnYesClick(object? sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void OnNoClick(object? sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
}

