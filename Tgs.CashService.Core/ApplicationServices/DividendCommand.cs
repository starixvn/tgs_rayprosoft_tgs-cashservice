using System;

namespace Tgs.CashService.Core.ApplicationServices
{
    public record DividendCommand(
        Guid BrandId,
        Guid WalletId,
        string PlayerCode,
        decimal Amount,
        string? Description,
        string? ExternalId);
}
