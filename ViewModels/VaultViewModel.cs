using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Password_manager.Data;
using Password_manager.Models;

namespace Password_manager.ViewModels;

public partial class VaultViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Website { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    public ObservableCollection<PasswordEntry> PasswordEntries { get; } = new()
    {
    };

    public ObservableCollection<PasswordEntry> FilteredPasswordEntries { get; } = new();

    public VaultViewModel()
    {
        LoadPasswords();
        RefreshFilteredEntries();
    }

    private void LoadPasswords()
    {
        PasswordEntries.Clear();

        if (!VaultSession.IsUnlocked)
        {
            return;
        }

        foreach (var entry in DatabaseMethods.GetPasswords())
        {
            PasswordEntries.Add(entry);
        }
    }

    partial void OnSearchQueryChanged(string value)
    {
        RefreshFilteredEntries();
    }

    private void RefreshFilteredEntries()
    {
        FilteredPasswordEntries.Clear();

        foreach (var entry in PasswordEntries.Where(EntryMatchesSearch))
        {
            FilteredPasswordEntries.Add(entry);
        }
    }

    private bool EntryMatchesSearch(PasswordEntry entry)
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            return true;
        }

        var query = SearchQuery.Trim();

        return entry.Website.Contains(query, StringComparison.OrdinalIgnoreCase)
            || entry.Username.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

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
            RefreshFilteredEntries();
        }
    }

    [RelayCommand]
    private void AddPasswordEntry()
    {
        if (string.IsNullOrWhiteSpace(Website) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            return;
        }
        DatabaseMethods.AddPassword(Website, Username, Password);
        LoadPasswords();
        RefreshFilteredEntries();
        Website = string.Empty;
        Username = string.Empty;
        Password = string.Empty;
    }

    [RelayCommand]
    private void ViewPassword(PasswordEntry entry)
    {
        if (entry != null)
        {
            entry.IsPasswordVisible = !entry.IsPasswordVisible;
        }
    }
}