using System;

namespace Tgs.CashService.Models
{
    public record TransactionViewModel
    {
        public Guid TransactionId { get; init; }
        public decimal Balance { get; init; }
    }
}
