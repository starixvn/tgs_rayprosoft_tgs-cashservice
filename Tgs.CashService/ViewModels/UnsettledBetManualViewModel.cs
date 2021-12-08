using System;

namespace Tgs.CashService.ViewModels
{
    public class UnsettledBetManualViewModel
    {
        public decimal Amount { get; set; }

        public string Description { get; set; } = default!;

        public string ExternalId { get; set; } = default!;

        public Guid CreatedByUserId { get; set; }
    }
}
