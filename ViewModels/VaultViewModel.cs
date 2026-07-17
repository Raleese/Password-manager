using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Password_manager.Models;

namespace Password_manager.ViewModels;

public partial class VaultViewModel : ViewModelBase
{
    public ObservableCollection<PasswordEntry> PasswordEntries { get; } = new()
    {
        new PasswordEntry { Website = "example.com", Username = "user1", Password = "password1" },
        new PasswordEntry { Website = "example.org", Username = "user2", Password = "password2" },
        new PasswordEntry { Website = "example.net", Username = "user3", Password = "password3" }
    };

    [RelayCommand]
    private void EditPasswordEntry(PasswordEntry entry)
    {
        // Implement the logic to edit the password entry
    }

    [RelayCommand]
    private void DeletePasswordEntry(PasswordEntry entry)
    {
        if (entry != null)
        {
            PasswordEntries.Remove(entry);
        }
    }
}