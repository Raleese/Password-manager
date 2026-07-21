using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Password_manager.Models;

namespace Password_manager.Data;

public static class MasterHash
{
    private const int SaltSize = 16; // Size of the salt in bytes
    private const int HashSize = 32; // Size of the hash in bytes
    private const int Iterations = 4; // Number of iterations for PBKDF2
    private const int MemorySize = 64 *1024;

    public static bool IsVaultInitialized()
    {
        // Ensure database and tables exist before checking
        DatabaseCreation.InitializeDatabase();

        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM VaultAuth WHERE Id = 1;";

        var count = (long)command.ExecuteScalar()!;
        return count > 0;
    }

    public static VaultAuthRecord SaveMasterPassword(string masterPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var encryptionSalt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashPassword(masterPassword, salt);
        var date = DateTime.Now;

        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO VaultAuth (Id, PasswordSalt, EncryptionSalt, PasswordHash, CreatedAt)
            VALUES (1, $salt, $encryptionSalt, $hash, $date)
            ON CONFLICT(Id) DO UPDATE SET
                PasswordSalt = excluded.PasswordSalt,
                EncryptionSalt = excluded.EncryptionSalt,
                PasswordHash = excluded.PasswordHash,
                CreatedAt = excluded.CreatedAt;
        """;
        command.Parameters.AddWithValue("$salt", salt);
        command.Parameters.AddWithValue("$encryptionSalt", encryptionSalt);
        command.Parameters.AddWithValue("$hash", hash);
        command.Parameters.AddWithValue("$date", date);
        command.ExecuteNonQuery();

        return new VaultAuthRecord
        {
            Id = 1,
            PasswordSalt = salt,
            EncryptionSalt = encryptionSalt,
            PasswordHash = hash,
            CreatedAt = date
        };
    }

    public static bool VerifyMasterPassword(string enteredPassword)
    {
        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT PasswordSalt, PasswordHash
            FROM VaultAuth
            WHERE Id = 1;
        """;

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return false;
        }

        var storedSalt = (byte[])reader["PasswordSalt"];
        var storedHash = (byte[])reader["PasswordHash"];

        var enteredHash = HashPassword(enteredPassword, storedSalt);

        return CryptographicOperations.FixedTimeEquals(storedHash, enteredHash);
    }

    public static byte[] HashPassword(string password, byte[] salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Environment.ProcessorCount,
            MemorySize = MemorySize,
            Iterations = Iterations,
        };
        return argon2.GetBytes(HashSize);
    }

    public static byte[] GetEncryptionSalt()
    {
        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT PasswordSalt, EncryptionSalt
            FROM VaultAuth
            WHERE Id = 1;
        """;

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            throw new InvalidOperationException("VaultAuth record not found.");
        }

        var encryptionSalt = (byte[])reader["EncryptionSalt"];
        return encryptionSalt;
    }
}