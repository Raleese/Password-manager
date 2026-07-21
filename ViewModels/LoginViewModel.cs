using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Password_manager.Data;
using System;

namespace Password_manager.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    public event Action? LoggedIn;

    [ObservableProperty]
    public partial string MasterPassword { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    [RelayCommand]
    private void Login()
    {
        if (string.IsNullOrEmpty(MasterPassword))
        {
            Message = "Enter your password.";
            return;
        }

        if (MasterHash.VerifyMasterPassword(MasterPassword))
        {
            var encryptionSalt = MasterHash.GetEncryptionSalt();
            var vaultKey = VaultCrypto.DeriveKey(MasterPassword, encryptionSalt);
            VaultSession.Unlock(vaultKey);
            LoggedIn?.Invoke();
            return;
        }

        Message = "Incorrect master password.";
    }
}