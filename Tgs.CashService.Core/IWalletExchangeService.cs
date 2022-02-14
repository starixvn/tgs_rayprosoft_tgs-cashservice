using Tgs.Cash.Protos;

namespace Tgs.CashService.Core;

internal interface IWalletExchangeService
{
    ValueTask<ExchangeResult> SendRequestAsync(WalletExchangeRequest request, CancellationToken cancellationToken = default);

    ValueTask<decimal?> GetBalanceAsync(Guid WalletId, CancellationToken cancellationToken = default);
}
