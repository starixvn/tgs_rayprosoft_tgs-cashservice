using System;

namespace Tgs.CashService.Client
{
    public record TransactionResponse
    {
        public Guid TransactionId { get; init; }

        public decimal Balance { get; init; }
    }
}
