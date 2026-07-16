using CommunityToolkit.Mvvm.ComponentModel;

namespace Password_manager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial ViewModelBase CurrentView { get; set; } = null!;

    public MainViewModel()
    {
        var createVaultViewModel = new CreateVaultViewModel();

        createVaultViewModel.VaultCreated += OpenVault;

        CurrentView = createVaultViewModel;
    }

    private void OpenVault()
    {
        CurrentView = new VaultViewModel();
    }
}