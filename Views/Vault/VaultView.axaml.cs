using Avalonia.Controls;
using Password_manager.Models;
using Password_manager.ViewModels;

namespace Password_manager.Views;

public partial class VaultView : UserControl
{
    public VaultView()
    {
        InitializeComponent();
    }

    private void EditButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is VaultViewModel viewModel && sender is Button button && button.DataContext is PasswordEntry entry)
        {
            viewModel.EditPasswordEntryCommand.Execute(entry);
        }
    }

    private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is VaultViewModel viewModel && sender is Button button && button.DataContext is PasswordEntry entry)
        {
            viewModel.DeletePasswordEntryCommand.Execute(entry);
        }
    }

    private void AddNewPasswordButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is VaultViewModel viewModel)
        {
            viewModel.AddPasswordEntryCommand.Execute(null);
        }
    }
    private void ViewButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is VaultViewModel viewModel && sender is Button button && button.DataContext is PasswordEntry entry)
        {
            viewModel.ViewPasswordCommand.Execute(entry);
        }
    }
}