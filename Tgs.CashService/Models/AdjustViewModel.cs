using System;

namespace Tgs.CashService.Models
{
    public class AdjustViewModel
    {
        public Guid BrandId { get; set; }

        public Guid WalletId { get; set; }

        public string PlayerCode { get; set; } = default!;

        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public string? ExternalId { get; set; }

        public Guid CreatedByUserId { get; set; }
    }
}
