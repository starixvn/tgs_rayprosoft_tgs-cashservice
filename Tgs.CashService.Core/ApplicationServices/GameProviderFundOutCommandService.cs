using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class GameProviderFundOutCommandService(IWalletExchangeService walletExchangeService)
    : IGetService<GameProviderFundOutCommand, ExchangeResult>
{
    public async ValueTask<ExchangeResult> GetAsync(GameProviderFundOutCommand command, CancellationToken cancellationToken = default)
    {
        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameProviderFundOutLock.ToString(),
            Amount = -command.Amount,
            LockAmount = command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        var lockResult = await walletExchangeService.SendRequestAsync(
            request,
            cancellationToken).ConfigureAwait(false);

        if (lockResult.Balance < 0M)
        {
            var rollbackRequest = new WalletExchangeRequest
            {
                WalletId = command.WalletId,
                TransactionType = TransactionType.GameProviderFundOutRollback.ToString(),
                Amount = command.Amount,
                LockAmount = -command.Amount,
                Description = command.Description ?? string.Empty,
                ExternalId = command.ExternalId ?? string.Empty
            };

            _ = await walletExchangeService.SendRequestAsync(
                rollbackRequest,
                cancellationToken).ConfigureAwait(false);

            throw new Exception("錢包餘額不足");
        }

        var commitRequest = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.GameProviderFundOutCommit.ToString(),
            Amount = 0,
            LockAmount = -command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return await walletExchangeService.SendRequestAsync(
            commitRequest,
            cancellationToken).ConfigureAwait(false);
    }
}
