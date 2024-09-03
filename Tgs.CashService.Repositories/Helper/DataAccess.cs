using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Tgs.CashService.Repositories
{
    internal interface IDbAcess 
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters);

        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object parameters);

        Task<int> ExecuteAsync(string sql, object parameters);
    }
    internal class DbAccess : IDbAcess
    {
        private readonly string connectionString;
        private readonly ILogger<DbAccess> logger;

        public DbAccess(IConfiguration configuration, ILogger<DbAccess> logger)
        {
            connectionString = configuration.GetConnectionString("Tgs")!;
            this.logger = logger;
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
