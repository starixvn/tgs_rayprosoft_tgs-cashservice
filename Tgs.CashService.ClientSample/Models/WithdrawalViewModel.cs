using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.CashService.ClientSample.Models
{
    public record WithdrawalViewModel
    {
        public Guid BrandId { get; init; }
        public Guid WalletId { get; init; }
        public string PlayerCode { get; init; } = default!;
        [Range(int.MinValue, 0)]
        public decimal Amount { get; init; }
        public string? Description { get; init; }
        public string? ExternalId { get; init; }
    }
}
