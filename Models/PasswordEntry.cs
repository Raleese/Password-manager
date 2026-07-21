using CommunityToolkit.Mvvm.ComponentModel;

namespace Password_manager.Models;

public partial class PasswordEntry : ObservableObject
{
    public int Id { get; set; }
    public string Website { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public byte[] Nonce { get; set; } = [];
    public byte[] Tag { get; set; } = [];
    public string Password { get; set; } = string.Empty;

    [ObservableProperty]
    private bool isPasswordVisible;

    public string DisplayPassword =>
        IsPasswordVisible ? Password : "**********";

    partial void OnIsPasswordVisibleChanged(bool value)
    {
        OnPropertyChanged(nameof(DisplayPassword));
    }
}