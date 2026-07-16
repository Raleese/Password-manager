using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Password_manager.ViewModels;

public partial class CreateVaultViewModel : ViewModelBase
{
    public event Action? VaultCreated;

    [ObservableProperty]
    public partial string MasterPassword { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConfirmMasterPassword { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    [RelayCommand]
    private void CreateVault()
    {
        if (string.IsNullOrEmpty(MasterPassword) || string.IsNullOrEmpty(ConfirmMasterPassword))
        {
            Message = "Please fill in both fields.";
            return;
        }

        if (MasterPassword != ConfirmMasterPassword)
        {
            Message = "Passwords do not match.";
            return;
        }

        VaultCreated?.Invoke();
    }
}