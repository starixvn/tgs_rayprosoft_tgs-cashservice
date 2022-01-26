using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices;

internal class GameProviderFundOutCommandService : IGetService<GameProviderFundOutCommand, ExchangeResult>
{
    private readonly IWalletRepository _walletRepository;
    private readonly WalletExchanger _walletExchanger;

    public GameProviderFundOutCommandService(IWalletRepository walletRepository, WalletExchanger walletExchanger)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _walletExchanger = walletExchanger ?? throw new ArgumentNullException(nameof(walletExchanger));
    }

    public async ValueTask<ExchangeResult> GetAsync(GameProviderFundOutCommand command, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository
            .GetWalletAsync(command.WalletId)
            .ConfigureAwait(false);

        if (wallet == null)
            throw new InvalidOperationException("Can't found wallet.");

        var request = new ExchangeRequest(
            command.WalletId,
            TransactionType.GameProviderFundOut,
            wallet.BrandId,
            wallet.PlayerCode,
            -command.Amount,
            command.Description,
            command.ExternalId);

        return await _walletExchanger.RequestAsync(request).ConfigureAwait(false);
    }
}
