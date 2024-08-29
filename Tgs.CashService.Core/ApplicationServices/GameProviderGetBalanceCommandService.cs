using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderGetBalanceCommandService(ITgsRepository tgsRepository)
    : IGetService<GameProviderGetBalanceCommand, BalanceResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderGetBalanceCommand command, CancellationToken cancellationToken = default)
    {
        var wallet = await _tgsRepository.GetWalletByPlayerCodeAsync(command.PlayerCode, cancellationToken).ConfigureAwait(false);

        if(wallet is null)
        {
            return new BalanceResult(Guid.Empty, 0, 0, ErrorCodes.NotFoundWallet);
        }
        return new BalanceResult(Guid.Empty, wallet.Balance, wallet.LockedBalance);

    }
}
