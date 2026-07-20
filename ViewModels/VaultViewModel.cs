using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Password_manager.Models;

namespace Password_manager.ViewModels;

public partial class VaultViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    public ObservableCollection<PasswordEntry> PasswordEntries { get; } = new()
    {
    };

    public ObservableCollection<PasswordEntry> FilteredPasswordEntries { get; } = new();

    public VaultViewModel()
    {
        RefreshFilteredEntries();
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
        // Implement the logic to add a new password entry
        RefreshFilteredEntries();
    }
}