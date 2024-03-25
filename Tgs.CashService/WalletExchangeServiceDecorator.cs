using NATS.Client.Core;
using Polly;
using Tgs.Cash.Protos;

namespace Tgs.CashService;

public class WalletExchangeServiceDecorator(
    IWalletExchangeService walletExchangeService,
    ILogger<WalletExchangeServiceDecorator> logger)
    : IWalletExchangeService
{
    public async ValueTask<ExchangeResult> SendRequestAsync(WalletExchangeRequest request, CancellationToken cancellationToken = default)
        => await Policy.Handle<NatsNoRespondersException>()
            .Or<NatsNoReplyException>()
            .RetryAsync(
                1,
                async (ex, count) =>
                {
                    await walletExchangeService.DiscardBidAsync(request.WalletId).ConfigureAwait(false);
                    logger.LogWarning(ex, "NoReponders error retry {count}", count);
                })
            .ExecuteAsync(
                async ct => await walletExchangeService
                    .SendRequestAsync(request, ct)
                    .ConfigureAwait(false),
                cancellationToken)
            .ConfigureAwait(false);

    public async ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
        => await Policy.Handle<NatsNoRespondersException>()
            .Or<NatsNoReplyException>()
            .RetryAsync(
                1,
                async (ex, count) =>
                {
                    await walletExchangeService.DiscardBidAsync(walletId).ConfigureAwait(false);
                    logger.LogWarning(ex, "NoReponders error retry {count}", count);
                })
            .ExecuteAsync(
                async ct => await walletExchangeService.GetBalanceAsync(
                    walletId,
                    cancellationToken).ConfigureAwait(false),
                cancellationToken)
            .ConfigureAwait(false);

    public ValueTask DiscardBidAsync(Guid walletId)
        => walletExchangeService.DiscardBidAsync(walletId);
}
