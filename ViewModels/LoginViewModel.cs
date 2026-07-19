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
        }

        if (MasterHash.VerifyMasterPassword(MasterPassword))
        {
            LoggedIn?.Invoke();
            return;
        }

        Message = "Incorrect master password.";
    }
}