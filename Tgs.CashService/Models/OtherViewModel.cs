using System;

namespace Tgs.CashService.Models
{
    public class OtherViewModel
    {
        public decimal Amount { get; set; }

        public string Description { get; set; } = default!;

        public string ExternalId { get; set; } = default!;

        public Guid CreatedByUserId { get; set; }
    }
}
