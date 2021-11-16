using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.Models
{
    public class BonusViewModel
    {
        public Guid BrandId { get; set; }

        public Guid WalletId { get; set; }

        public string PlayerCode { get; set; } = default!;

        [Range(0, int.MaxValue)]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public string? ExternalId { get; set; }
    }
}
