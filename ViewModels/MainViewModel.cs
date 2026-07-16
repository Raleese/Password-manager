namespace Password_manager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public CreateVaultViewModel CreateVault { get; } = new();
}
