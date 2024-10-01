using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Tgs.CashService.Repositories
{
    public interface IDbAcess
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters);

        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object parameters);

        Task<int> ExecuteAsync(string sql, object parameters);
    }

    public class DbAccess : IDbAcess
    {
        private readonly string connectionString;

        public DbAccess(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Tgs")!;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                return await connection.QueryAsync<T>(sql, parameters).ConfigureAwait(false);
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object parameters)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters).ConfigureAwait(false);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object parameters)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                return await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
            }
        }
    }
}
