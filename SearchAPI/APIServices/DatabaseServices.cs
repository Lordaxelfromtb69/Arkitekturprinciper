using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class DatabaseService
{
    private const string DatabasePath = @"C:\Data\index.db"; // Sti til den uploadede database

    /// <summary>
    /// Søger i databasen efter dokumenttitler, hvor ordet matcher søgeforespørgslen.
    /// </summary>
    public List<string> Search(string query)
    {
        var results = new List<string>();

        using var connection = new SqliteConnection($"Data Source={DatabasePath};");
        connection.Open();

        string sql = "SELECT DISTINCT name FROM word WHERE name LIKE @query";
        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@query", $"%{query}%");

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(reader.GetString(0));
        }

        return results;
    }
}
