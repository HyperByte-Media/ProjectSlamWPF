using Dapper;
using System.Data;

namespace ProjectSlam.Data.Repositories;

public abstract class BaseRepository
{

    protected readonly DbConfig _dbConfig;

    protected BaseRepository(DbConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
    {
        using var connection = _dbConfig.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(sql, param);
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        using var connection = _dbConfig.CreateConnection();
        var result = await connection.QueryAsync<T>(sql, param);
        return result ?? Enumerable.Empty<T>();
    }

    protected async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        using var connection = _dbConfig.CreateConnection();
        return await connection.ExecuteAsync(sql, param);
    }

    protected async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null)
    {
        using var connection = _dbConfig.CreateConnection();
        return await connection.ExecuteScalarAsync<T>(sql, param);
    }

    protected async Task<IEnumerable<T>> QueryWithTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<IEnumerable<T>>> query)
    {
        using var connection = _dbConfig.CreateConnection();
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();
        try
        {
            var result = await query(connection, transaction);
            transaction.Commit();
            return result ?? Enumerable.Empty<T>();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
