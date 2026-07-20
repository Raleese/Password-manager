using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using System.Text;

namespace Password_manager.Data;

public static class VaultCrypto
{
    public static (byte[] Nonce, byte[] Ciphertext, byte[] Tag) Encrypt(string plaintext, byte[] key)
    {
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        byte[] nonce = RandomNumberGenerator.GetBytes(12); // 12 bytes for AES-GCM nonce
        byte[] ciphertext = new byte[plaintextBytes.Length];
        int tagSize = 16; // 16 bytes for AES-GCM tag
        byte[] tag = new byte[tagSize]; // 16 bytes for AES-GCM tag

        using var aesGcm = new AesGcm(key, tagSize);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        return (nonce, ciphertext, tag);
    }

    public static string Decrypt(byte[] nonce, byte[] ciphertext, byte[] tag, byte[] key)
    {
        byte[] plaintext = new byte[ciphertext.Length];

        using var aesGcm = new AesGcm(key, tag.Length);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }

    public static byte[] DeriveKey(string masterPassword, byte[] salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(masterPassword))
        {
            Salt = salt,
            DegreeOfParallelism = 8, // Number of threads to use
            MemorySize = 64 * 1024, // 64 MB
            Iterations = 4 // Number of iterations
        };

        return argon2.GetBytes(32); // Return 32 bytes (256 bits) for the encryption key
    }
}