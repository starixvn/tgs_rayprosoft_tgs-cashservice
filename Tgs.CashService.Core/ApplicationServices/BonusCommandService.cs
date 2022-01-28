using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices;

internal class BonusCommandService : IGetService<BonusCommand, ExchangeResult>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletExchangeService _walletExchangeService;

    public BonusCommandService(IWalletRepository walletRepository, IWalletExchangeService walletExchangeService)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(BonusCommand command, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository
            .GetWalletAsync(command.WalletId)
            .ConfigureAwait(false);

        if (wallet == null)
            throw new InvalidOperationException("Can't found wallet.");

        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.Bonus.ToString(),
            BrandId = wallet.BrandId,
            PlayerCode = wallet.PlayerCode,
            Amount = command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return await _walletExchangeService.SendRequestAsync(request).ConfigureAwait(false);
    }
}
