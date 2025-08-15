using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProjectSlam.Data;

public class DbConfig
{
    private readonly string _databasePath;

    public DbConfig()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _databasePath = Path.Combine(appDataPath, "ProjectSlamData", "ProjectSlam.db");

        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(_databasePath)!);
    }

    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(ConnectionString);
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            // Always run initialization to ensure tables exist
            var sqlPath = Path.Combine(AppContext.BaseDirectory, "DbInit.sql");

            if (!File.Exists(sqlPath))
                throw new FileNotFoundException($"Database initialization file not found at: {sqlPath}");
            
            var sql = await File.ReadAllTextAsync(sqlPath);

            if (string.IsNullOrWhiteSpace(sql))
                throw new InvalidOperationException("Database initialization SQL file is empty");            

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex) 
        {
            throw new Exception($"Failed to initialize database. Path: {_databasePath}, Error: {ex.Message}", ex);
        }
    }

    public string ConnectionString => $"Data Source={_databasePath};";
}
