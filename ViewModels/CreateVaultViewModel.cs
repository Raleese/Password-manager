using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Password_manager.ViewModels;

public partial class CreateVaultViewModel : ViewModelBase
{
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

        // Here you would add the logic to create the vault with the master password.
        // For now, we'll just set a success message.
        Message = "Vault created successfully!";
    }
}