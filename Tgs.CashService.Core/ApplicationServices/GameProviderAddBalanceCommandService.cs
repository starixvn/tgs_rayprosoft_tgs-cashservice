using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderAddBalanceCommandService(IWalletExchangeService walletExchangeService, ITgsRepository tgsRepository)
    : IGetService<GameProviderAddBalanceCommand, BalanceResult>
{
    private readonly IWalletExchangeService _walletExchangeService = walletExchangeService
        ?? throw new ArgumentNullException(nameof(walletExchangeService));

    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BalanceResult> GetAsync(GameProviderAddBalanceCommand command, CancellationToken cancellationToken = default)
    {
        Guid walletId = await tgsRepository.GetWalletIdAsync(command.PlayerCode, command.WalletId, cancellationToken).ConfigureAwait(false);
        var request = new WalletExchangeRequest
        {
            WalletId = walletId,
            TransactionType = TransactionType.GameProviderAddBalance.ToString(),
            Amount = command.Amount,
            LockAmount = 0M,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        var response = await _walletExchangeService
            .SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return new BalanceResult(response.TransactionId, response.Balance, response.LockedBalance, 0, string.Empty);
    }
}
