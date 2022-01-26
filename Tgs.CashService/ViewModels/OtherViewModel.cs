using System;

namespace Tgs.CashService.ViewModels
{
    public record OtherViewModel
    {
        public decimal Amount { get; init; }

        public string Description { get; init; } = default!;

        public string ExternalId { get; init; } = default!;

        public Guid CreatedByUserId { get; init; }
    }
}
