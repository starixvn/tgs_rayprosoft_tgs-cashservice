using Dapper;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Repositories.Entities;

namespace Tgs.CashService.Repositories;

internal class DbScriptExecutor : IDbScriptExecutor
{
    private readonly TgsContext _tgsContext;

    public DbScriptExecutor(TgsContext tgsContext)
    {
        _tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
    }

    public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? parameters)
    {
        var connection = _tgsContext.Database.GetDbConnection();

        return connection.QueryFirstOrDefaultAsync<T>(sql, parameters)!;
    }
}
