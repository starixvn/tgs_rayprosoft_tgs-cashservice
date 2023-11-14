namespace Tgs.CashService.Repositories;

public interface IDbScriptExecutor
{
    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? parameters);
}
