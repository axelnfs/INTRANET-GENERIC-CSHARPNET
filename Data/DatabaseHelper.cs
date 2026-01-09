using System.Data;
using Microsoft.Data.SqlClient;
// O usa: using Microsoft.Data.SqlClient;
using Dapper;

namespace INTRANET_GENERIC.Data;

public static class DatabaseHelper
{
    private static string _connectionString;

    public static void Initialize(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    private static IDbConnection GetConnection()
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new InvalidOperationException("DatabaseHelper no ha sido inicializado. Llama a Initialize() primero.");

        return new SqlConnection(_connectionString);
    }

    /// <summary>
    /// Ejecuta un stored procedure y retorna múltiples registros
    /// </summary>
    public static async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(
        string storedProcedure,
        object parameters = null,
        int? commandTimeout = null)
    {
        try
        {
            using var connection = GetConnection();
            return await connection.QueryAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Error ejecutando SP '{storedProcedure}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta un stored procedure y retorna un único registro
    /// </summary>
    public static async Task<T> ExecuteStoredProcedureSingleAsync<T>(
        string storedProcedure,
        object parameters = null,
        int? commandTimeout = null)
    {
        try
        {
            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Error ejecutando SP '{storedProcedure}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta un stored procedure sin retornar datos (INSERT, UPDATE, DELETE)
    /// </summary>
    public static async Task<int> ExecuteStoredProcedureNonQueryAsync(
        string storedProcedure,
        object parameters = null,
        int? commandTimeout = null)
    {
        try
        {
            using var connection = GetConnection();
            return await connection.ExecuteAsync(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Error ejecutando SP '{storedProcedure}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta un stored procedure y retorna un escalar (ej: COUNT, nuevo ID)
    /// </summary>
    public static async Task<T> ExecuteStoredProcedureScalarAsync<T>(
        string storedProcedure,
        object parameters = null,
        int? commandTimeout = null)
    {
        try
        {
            using var connection = GetConnection();
            return await connection.ExecuteScalarAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Error ejecutando SP '{storedProcedure}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta múltiples queries en una transacción
    /// </summary>
    public static async Task<bool> ExecuteTransactionAsync(Func<IDbConnection, IDbTransaction, Task> action)
    {
        using var connection = GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await action(connection, transaction);
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception($"Error en transacción: {ex.Message}", ex);
        }
    }
}