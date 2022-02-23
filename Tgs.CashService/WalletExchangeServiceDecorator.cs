using NATS.Client;
using Polly;
using Tgs.Cash.Protos;

namespace Tgs.CashService;

public class WalletExchangeServiceDecorator : IWalletExchangeService
{
    private readonly IWalletExchangeService _walletExchangeService;
    private readonly ILogger<WalletExchangeServiceDecorator> _logger;

    public WalletExchangeServiceDecorator(IWalletExchangeService walletExchangeService, ILogger<WalletExchangeServiceDecorator> logger)
    {
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<ExchangeResult> SendRequestAsync(WalletExchangeRequest request, CancellationToken cancellationToken = default)
        => await Policy.Handle<NATSNoRespondersException>()
            .RetryAsync(
                1,
                async (ex, count) =>
                {
                    await _walletExchangeService.DiscardBidAsync(request.WalletId).ConfigureAwait(false);
                    _logger.LogWarning(ex, $"NoReponders error retry {count}");
                })
            .ExecuteAsync(
                async ct => await _walletExchangeService
                    .SendRequestAsync(request, ct)
                    .ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);

    public ValueTask<decimal?> GetBalanceAsync(Guid WalletId, CancellationToken cancellationToken = default)
        => _walletExchangeService.GetBalanceAsync(WalletId, cancellationToken);

    public ValueTask DiscardBidAsync(Guid walletId)
        => _walletExchangeService.DiscardBidAsync(walletId);
}
