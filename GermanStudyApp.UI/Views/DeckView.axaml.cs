using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using GermanStudyApp.Core.Models;
using GermanStudyApp.UI.ViewModels;

namespace GermanStudyApp.UI.Views;

public partial class DeckView : UserControl
{
    // 드래그 앤 드롭으로 옮길 때, "이 덱을 옮기는 중"이라는 걸 담아두는 커스텀 데이터 포맷 이름.
    private const string DeckDragFormat = "GermanStudyApp.Deck";

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

    // 각 덱 행(Border)이 화면에 처음 나타날 때, 코드로 직접 Drop 이벤트 핸들러를 붙인다.
    // (DataTemplate 안의 요소는 XAML에서 DragDrop.Drop="..."으로 바로 연결이 안 되기 때문)
    private void OnDeckRowAttached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Border border)
        {
            DragDrop.SetAllowDrop(border, true);
            border.RemoveHandler(DragDrop.DropEvent, (EventHandler<DragEventArgs>)OnDeckRowDrop);
            border.AddHandler(DragDrop.DropEvent, (EventHandler<DragEventArgs>)OnDeckRowDrop);
        }
    }

    // 덱 행을 누른 채로 마우스를 움직이기 시작하면, 이 행을 "드래그 중인 물건"으로 등록한다.
    private async void OnDeckRowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border border || border.DataContext is not DeckDisplayItem item)
        {
            return;
        }

        if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
        {
            return;
        }

        var data = new DataObject();
        data.Set(DeckDragFormat, item.Source);

        await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
    }

    // 다른 덱 행 위에 드래그 중이던 덱을 놓으면, 그 덱을 이 행의 덱 밑으로 옮긴다.
    private async void OnDeckRowDrop(object? sender, DragEventArgs e)
    {
        if (sender is not Border border || border.DataContext is not DeckDisplayItem targetItem)
        {
            return;
        }

        if (e.Data.Get(DeckDragFormat) is not Deck draggedDeck)
        {
            return;
        }

        if (DataContext is DeckViewModel vm)
        {
            await vm.MoveDeckAsync(draggedDeck, targetItem.Source);
        }
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
