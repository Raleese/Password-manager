using CommunityToolkit.Mvvm.ComponentModel;
using Password_manager.Data;

namespace Password_manager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial ViewModelBase CurrentView { get; set; } = null!;

    public MainViewModel()
    {
        if (MasterHash.IsVaultInitialized())
        {
            var loginViewModel = new LoginViewModel();
            loginViewModel.LoggedIn += OpenVault;
            CurrentView = loginViewModel;
        }
        else
        {
            var createVaultViewModel = new CreateVaultViewModel();
            createVaultViewModel.VaultCreated += OpenLogin;
            CurrentView = createVaultViewModel;
        }
    }


    private void OpenVault()
    {
        CurrentView = new VaultViewModel();
    }

    private void OpenLogin()
    {
        var loginViewModel = new LoginViewModel();
        loginViewModel.LoggedIn += OpenVault;
        CurrentView = loginViewModel;
    }
}