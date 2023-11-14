namespace Tgs.CashService.Repositories;

internal class WalletBalanceReturn
{
    public decimal OriginBalance { get; init; }

    public decimal OriginLockedBalance { get; init; }

    public decimal PostBalance { get; init; }

    public decimal PostLockedBalance { get; init; }
}
