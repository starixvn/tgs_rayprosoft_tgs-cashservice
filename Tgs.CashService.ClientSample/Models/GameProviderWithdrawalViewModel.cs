using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.ClientSample.Models
{
    public record GameProviderWithdrawalViewModel
    {
        public Guid BrandId { get; init; }
        public Guid WalletId { get; init; }
        public string PlayerCode { get; init; } = default!;
        [Range(0, int.MaxValue)]
        public decimal Amount { get; init; }
        public string? Description { get; init; }
        public string? ExternalId { get; init; }
    }
}
