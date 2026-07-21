using System;
using System.Security.Cryptography;

namespace Password_manager.Data;

public static class VaultSession
{
    private static byte[]? vaultKey;

    public static void Unlock(byte[] key)
    {
        vaultKey = key;
    }

    public static byte[] GetKey()
    {
        return vaultKey ?? throw new InvalidOperationException("Vault is not unlocked.");
    }

    public static bool IsUnlocked => vaultKey is not null;

    public static void Lock()
    {
        if (vaultKey is not null)
        {
            CryptographicOperations.ZeroMemory(vaultKey);
            vaultKey = null;
        }
    }
}