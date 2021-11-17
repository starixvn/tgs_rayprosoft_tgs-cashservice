using System;

namespace Tgs.CashService.Models
{
    public class TransactionResultViewModel
    {
        public Guid TransactionId { get; set; }

        public decimal Balance { get; set; }
    }
}
