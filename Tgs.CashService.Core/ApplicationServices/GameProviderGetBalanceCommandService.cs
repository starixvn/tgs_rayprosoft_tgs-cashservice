using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderGetBalanceCommandService(IWalletExchangeService walletExchangeService, ITgsRepository tgsRepository)
    : IGetService<GameProviderGetBalanceCommand, BalanceResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderGetBalanceCommand command, CancellationToken cancellationToken = default)
    {
        var wallet = await _tgsRepository.GetWalletByPlayerCodeAsync(command.PlayerCode, cancellationToken).ConfigureAwait(false);

        if(wallet is null)
        {
            return new BalanceResult(Guid.Empty, 0, 0, ErrorCodes.NotFoundWallet);
        }
        return new BalanceResult(Guid.Empty, wallet.Balance, 0);

    }
}
