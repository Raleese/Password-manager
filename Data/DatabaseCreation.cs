using Microsoft.Data.Sqlite;
using System.IO;
using System;

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
                CreatedAt DATETIME NOT NULL,
            );
            """;
        createTableCommand.ExecuteNonQuery();

        var createVaultAuthTableCommand = connection.CreateCommand();
        createVaultAuthTableCommand.CommandText = """
            CREATE TABLE IF NOT EXISTS VaultAuth (
                Id INTEGER PRIMARY KEY CHECK (Id = 1),
                Salt BLOB NOT NULL,
                Hash BLOB NOT NULL,
                CreatedAt DATETIME NOT NULL
            );
            """;
        createVaultAuthTableCommand.ExecuteNonQuery();
    }
}