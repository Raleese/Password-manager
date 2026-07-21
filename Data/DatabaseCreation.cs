using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Password_manager.Data;

public static class DatabaseCreation
{
    private static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Password-manager", "password-manager.db");
   
    private static readonly string ConnectionString = new SqliteConnectionStringBuilder
    {
        DataSource = DatabasePath
    }.ToString();

    public static SqliteConnection CreateConnection()
    {
        var directory = Path.GetDirectoryName(DatabasePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return new SqliteConnection(ConnectionString);
    }

    public static void InitializeDatabase()
    {
        using var connection = CreateConnection();
        connection.Open();

        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText = """
            CREATE TABLE IF NOT EXISTS PasswordEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Website TEXT NOT NULL,
                Username TEXT NOT NULL,
                Nonce BLOB NOT NULL,
                Tag BLOB NOT NULL,
                Password BLOB NOT NULL,
                CreatedAt DATETIME NOT NULL
            );
            """;
        createTableCommand.ExecuteNonQuery();

        var createVaultAuthTableCommand = connection.CreateCommand();
        createVaultAuthTableCommand.CommandText = """
            CREATE TABLE IF NOT EXISTS VaultAuth (
                Id INTEGER PRIMARY KEY CHECK (Id = 1),
                PasswordSalt BLOB NOT NULL,
                EncryptionSalt BLOB NOT NULL,
                PasswordHash BLOB NOT NULL,
                CreatedAt DATETIME NOT NULL
            );
            """;
        createVaultAuthTableCommand.ExecuteNonQuery();

        MigrateVaultAuthSchema(connection);
    }

    private static void MigrateVaultAuthSchema(SqliteConnection connection)
    {
        var columns = new List<string>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA table_info(VaultAuth);";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(reader.GetString(1));
            }
        }

        if (columns.Count == 0 || columns.Contains("PasswordSalt"))
        {
            return;
        }

        if (columns.Contains("Salt"))
        {
            using var renameSalt = connection.CreateCommand();
            renameSalt.CommandText = "ALTER TABLE VaultAuth RENAME COLUMN Salt TO PasswordSalt;";
            renameSalt.ExecuteNonQuery();
        }

        if (columns.Contains("Hash"))
        {
            using var renameHash = connection.CreateCommand();
            renameHash.CommandText = "ALTER TABLE VaultAuth RENAME COLUMN Hash TO PasswordHash;";
            renameHash.ExecuteNonQuery();
        }
    }
}