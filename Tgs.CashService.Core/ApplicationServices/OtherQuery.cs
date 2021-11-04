using System;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record OtherQuery
    {
        public Guid BrandId { get; init; }
        public Guid WalletId { get; init; }
        public string PlayerCode { get; init; } = default!;
        public decimal Amount { get; init; }
        public string Description { get; init; } = default!;
        public string ExternalId { get; init; } = default!;
        public Guid CreatedByUserId { get; init; }
    }
}
