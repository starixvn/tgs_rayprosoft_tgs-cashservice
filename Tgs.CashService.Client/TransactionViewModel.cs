using System;

namespace Tgs.CashService.Client
{
    public record TransactionViewModel
    {
        public Guid TransactionId { get; init; }
        public decimal Balance { get; init; }
    }
}
