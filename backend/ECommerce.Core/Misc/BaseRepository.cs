using Npgsql;
using System.Data;

namespace ECommerce.Core.Misc;

public abstract class BaseRepository
{
    protected readonly string _connectionString;

    protected BaseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected async Task<NpgsqlConnection> GetConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    protected async Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        
        if (parameters != null)
        {
            AddParameters(command, parameters);
        }

        var result = await command.ExecuteScalarAsync();
        return result != null && result != DBNull.Value ? (T)result : default;
    }

    protected async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        
        if (parameters != null)
        {
            AddParameters(command, parameters);
        }

        return await command.ExecuteNonQueryAsync();
    }

    protected async Task<List<T>> QueryAsync<T>(string sql, Func<IDataReader, T> mapper, object? parameters = null)
    {
        var results = new List<T>();
        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        
        if (parameters != null)
        {
            AddParameters(command, parameters);
        }

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(mapper(reader));
        }

        return results;
    }

    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, Func<IDataReader, T> mapper, object? parameters = null)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        
        if (parameters != null)
        {
            AddParameters(command, parameters);
        }

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return mapper(reader);
        }

        return default;
    }

    private void AddParameters(NpgsqlCommand command, object parameters)
    {
        var properties = parameters.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var parameter = new NpgsqlParameter($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value);
            command.Parameters.Add(parameter);
        }
    }

    protected object GetValueOrDbNull(object? value)
    {
        return value ?? DBNull.Value;
    }
}
