using System;

namespace Tgs.CashService.Core
{
    public record ExchangeResult(
        Guid TransactionId,
        decimal Balance);
}
