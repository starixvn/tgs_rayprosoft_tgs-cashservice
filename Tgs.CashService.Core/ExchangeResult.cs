using System;

namespace Tgs.CashService.Core
{
    public record ExchangeResult
    {
        public Guid TransactionId { get; init; }
        public decimal Balance { get; init; }
    }
}
