using Tgs.Cash.Protos;

namespace Tgs.CashService;

public interface IWalletExchangeService
{
    ValueTask DiscardBidAsync(Guid walletId);

    ValueTask<ExchangeResult> SendRequestAsync(WalletExchangeRequest request, CancellationToken cancellationToken = default);

    ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default);
}
