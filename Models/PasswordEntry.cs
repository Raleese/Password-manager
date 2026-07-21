namespace Password_manager.Models;

public class PasswordEntry
{
    public int Id { get; set; }
    public string Website { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public byte[] Nonce { get; set; } = [];
    public byte[] Tag { get; set; } = [];
    public string Password { get; set; } = string.Empty;
}