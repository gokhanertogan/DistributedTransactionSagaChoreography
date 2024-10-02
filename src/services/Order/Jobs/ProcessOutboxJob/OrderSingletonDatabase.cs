using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ProcessOutboxJob;

public static class OrderSingletonDatabase
{
    static OrderSingletonDatabase()
    {
        _connection = new SqlConnection("Server=localhost,1433;Database=ChoreographyOutboxOrderDB;User ID=SA;Password=Gkn12345678*;TrustServerCertificate=True;");
    }

    static IDbConnection _connection;
    public static IDbConnection Connection
    {
        get
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            return _connection;
        }
    }

    public static async Task<IEnumerable<T>> QueryAsync<T>(string sql)
        => await _connection.QueryAsync<T>(sql);
    public static async Task<int> ExecuteAsync(string sql)
        => await _connection.ExecuteAsync(sql);

    static bool _dataReaderState = true;
    public static bool DataReaderState { get => _dataReaderState; }

    public static void DataReaderReady() => _dataReaderState = true;
    public static void DataReaderBusy() => _dataReaderState = false;
}