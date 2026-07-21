using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using Password_manager.Models;

namespace Password_manager.Data;

public static class DatabaseMethods
{
    public static void AddPassword(string website, string username, string entryPassword)
    {
        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        var vaultKey = VaultSession.GetKey();
        var (nonce, ciphertext, tag) = VaultCrypto.Encrypt(entryPassword, vaultKey);
        var createdAt = DateTime.UtcNow;

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO PasswordEntries (Website, Username, Nonce, Tag, Password, CreatedAt)
            VALUES ($website, $username, $nonce, $tag, $password, $createdAt);
        """;
        command.Parameters.AddWithValue("$website", website);
        command.Parameters.AddWithValue("$username", username);
        command.Parameters.AddWithValue("$nonce", nonce);
        command.Parameters.AddWithValue("$tag", tag);
        command.Parameters.AddWithValue("$password", ciphertext);
        command.Parameters.AddWithValue("$createdAt", createdAt);
        command.ExecuteNonQuery();
    }

    public static List<PasswordEntry> GetPasswords()
    {
        var entries = new List<PasswordEntry>();
        var vaultKey = VaultSession.GetKey();

        using var connection = DatabaseCreation.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Website, Username, Nonce, Tag, Password
            FROM PasswordEntries
            ORDER BY CreatedAt DESC, Id DESC;
        """;

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var nonce = (byte[])reader["Nonce"];
            var tag = (byte[])reader["Tag"];
            var ciphertext = (byte[])reader["Password"];

            entries.Add(new PasswordEntry
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Website = reader.GetString(reader.GetOrdinal("Website")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Password = VaultCrypto.Decrypt(nonce, ciphertext, tag, vaultKey)
            });
        }

        return entries;
    }
}