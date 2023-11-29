using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class NewCashCommandService(
    ITgsRepository tgsRepository,
    IWalletExchangeService walletExchangeService)
    : IGetService<NewCashCommand, Guid>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<Guid> GetAsync(NewCashCommand command, CancellationToken cancellationToken = default)
    {
        var walletId = Guid.NewGuid();

        await _tgsRepository.CreateWalletAsync(
            walletId,
            command.BrandId,
            command.UserId,
            command.Type,
            command.CurrencyCode,
            cancellationToken).ConfigureAwait(false);

        var request = new WalletExchangeRequest
        {
            WalletId = walletId,
            TransactionType = nameof(TransactionType.CreateWallet),
            Amount = 0M,
            LockAmount = 0M,
            Description = "Create wallet"
        };

        _ = await walletExchangeService
            .SendRequestAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return walletId;
    }
}
