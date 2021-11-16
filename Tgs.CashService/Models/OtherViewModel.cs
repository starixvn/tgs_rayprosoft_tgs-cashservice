using System;

namespace Tgs.CashService.Models
{
    public class OtherViewModel
    {
        public Guid BrandId { get; set; }

        public Guid WalletId { get; set; }

        public string PlayerCode { get; set; } = default!;

        public decimal Amount { get; set; }

        public string Description { get; set; } = default!;

        public string ExternalId { get; set; } = default!;

        public Guid CreatedByUserId { get; set; }
    }
}
