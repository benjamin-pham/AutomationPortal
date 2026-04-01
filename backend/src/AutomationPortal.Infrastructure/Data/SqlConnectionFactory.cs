using System.Data;
using Npgsql;
using AutomationPortal.Application.Abstractions.Data;

namespace AutomationPortal.Infrastructure.Data;

internal sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
