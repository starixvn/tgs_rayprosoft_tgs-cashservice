using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices;

public class GameProviderFundInService : IGetService<GameProviderFundInCommand, ExchangeResult>
{
    private readonly IWalletRepository _walletRepository;

    public GameProviderFundInService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
    }

    public async ValueTask<ExchangeResult> GetAsync(GameProviderFundInCommand command, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetWalletAsync(command.WalletId);

        if (wallet == null)
            throw new InvalidOperationException("Can't found wallet.");

        var (transactionId, postBalance) = await _walletRepository
            .ExchangeAsync(
                wallet.BrandId,
                command.WalletId,
                wallet.PlayerCode,
                TransactionType.GameProviderFundIn,
                command.Amount,
                command.Description,
                command.ExternalId)
            .ConfigureAwait(false);

        return new ExchangeResult(transactionId, postBalance);
    }
}
