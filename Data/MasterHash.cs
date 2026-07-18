using System;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Password_manager.Models;

namespace Password_manager.Data;

public static class MasterHash
{
    private const int SaltSize = 16; // Size of the salt in bytes
    private const int HashSize = 32; // Size of the hash in bytes
    private const int Iterations = 100000; // Number of iterations for PBKDF2

    public static bool IsVaultInitialized()
    {
        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM VaultAuth;";

        var count = (long)command.ExecuteScalar()!;
        return count > 0;
    }

    public static void SaveMasterPassword(string masterPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashPassword(masterPassword, salt);
        var date = DateTime.Now;

        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO VaultAuth (Id, Salt, Hash, CreatedAt)
            VALUES (1, $salt, $hash, $date)
            ON CONFLICT(Id) DO UPDATE SET
                Salt = excluded.Salt,
                Hash = excluded.Hash,
                CreatedAt = excluded.CreatedAT;
        """;
        command.Parameters.AddWithValue("$salt", salt);
        command.Parameters.AddWithValue("$hash", hash);
        command.Parameters.AddWithValue("$date", date);
        command.ExecuteNonQuery();
    }

    public static bool VerifyMasterPassword(string enteredPassword)
    {
        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Salt, Hash
            FROM VaultAuth
            WHERE Id = 1;
        """;

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return false;
        }

        var storedHash = (byte[])reader["Salt"];
        var storedSalt = (byte[])reader["Hash"];

        var enteredHash = HashPassword(enteredPassword, storedSalt);

        return CryptographicOperations.FixedTimeEquals(storedHash, enteredHash);
    }

    public static byte[] HashPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
    }
}