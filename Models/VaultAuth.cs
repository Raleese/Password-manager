using System;

namespace Password_manager.Models;

public sealed class VaultAuthRecord
{
    public int Id { get; set; }
    public byte[] PasswordSalt { get; set; } = [];
    public byte[] EncryptionSalt { get; set; } = [];
    public byte[] PasswordHash { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}