using System;

namespace Tgs.CashService.Core
{
    public record BalanceQuery
    {
        public Guid WalletId { get; init; }
    }
}
