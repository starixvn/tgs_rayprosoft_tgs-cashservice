using NATS.Client.Core;
using Polly;
using Tgs.Cash.Protos;

namespace Tgs.CashService;

public class WalletExchangeServiceDecorator(
    IWalletExchangeService walletExchangeService,
    ILogger<WalletExchangeServiceDecorator> logger)
    : IWalletExchangeService
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    private readonly ILogger<WalletExchangeServiceDecorator> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    public async ValueTask<ExchangeResult> SendRequestAsync(WalletExchangeRequest request, CancellationToken cancellationToken = default)
        => await Policy.Handle<NatsNoRespondersException>()
            .RetryAsync(
                1,
                async (ex, count) =>
                {
                    await _walletExchangeService.DiscardBidAsync(request.WalletId).ConfigureAwait(false);
                    _logger.LogWarning(ex, "NoReponders error retry {count}", count);
                })
            .ExecuteAsync(
                async ct => await _walletExchangeService
                    .SendRequestAsync(request, ct)
                    .ConfigureAwait(false),
                cancellationToken)
            .ConfigureAwait(false);

    public async ValueTask<decimal?> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
        => await Policy.Handle<NatsNoRespondersException>()
            .RetryAsync(
                1,
                async (ex, count) =>
                {
                    await _walletExchangeService.DiscardBidAsync(walletId).ConfigureAwait(false);
                    _logger.LogWarning(ex, "NoReponders error retry {count}", count);
                })
            .ExecuteAsync(
                async ct => await _walletExchangeService.GetBalanceAsync(
                    walletId,
                    cancellationToken).ConfigureAwait(false),
                cancellationToken)
            .ConfigureAwait(false);

    public ValueTask DiscardBidAsync(Guid walletId)
        => _walletExchangeService.DiscardBidAsync(walletId);
}
