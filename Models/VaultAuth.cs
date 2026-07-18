using System;

namespace Password_manager.Models;

public sealed class VaultAuthRecord
{
    public int Id { get; set; }
    public byte[] Salt { get; set; } = [];
    public byte[] Hash { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}