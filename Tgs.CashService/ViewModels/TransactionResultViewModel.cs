using System;

namespace Tgs.CashService.ViewModels
{
    public class TransactionResultViewModel
    {
        public Guid TransactionId { get; set; }

        public decimal Balance { get; set; }
    }
}
