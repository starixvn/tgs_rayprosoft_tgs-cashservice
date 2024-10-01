using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderGetBalanceCommandService(ITgsRepository tgsRepository)
    : IGetService<GameProviderGetBalanceCommand, BalanceResult>
{
    public async ValueTask<BalanceResult> GetAsync(GameProviderGetBalanceCommand command, CancellationToken cancellationToken = default)
    {
        var wallet = await tgsRepository.GetWalletByPlayerCodeAsync(command.PlayerCode, cancellationToken).ConfigureAwait(false);

        if (wallet is null)
        {
            return new BalanceResult(Guid.Empty, 0, 0, ErrorCode.NotFoundWallet);
        }
        return new BalanceResult(Guid.Empty, wallet.Balance, wallet.LockedBalance);
    }
}
