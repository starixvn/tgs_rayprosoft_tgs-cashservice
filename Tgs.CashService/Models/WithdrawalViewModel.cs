using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.Models
{
    public class WithdrawalViewModel
    {
        public Guid BrandId { get; set; }

        public Guid WalletId { get; set; }

        public string PlayerCode { get; set; } = default!;

        [Range(int.MinValue, 0)]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public string? ExternalId { get; set; }
    }
}
