using Tgs.Cash.Protos;
using Tgs.ServiceUtility;

namespace Tgs.CashService.Core.ApplicationServices;

internal class WithdrawalCommandService : IGetService<WithdrawalCommand, ExchangeResult>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletExchangeService _walletExchangeService;

    public WithdrawalCommandService(IWalletRepository walletRepository, IWalletExchangeService walletExchangeService)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
    }

    public async ValueTask<ExchangeResult> GetAsync(WithdrawalCommand command, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository
            .GetWalletAsync(command.WalletId)
            .ConfigureAwait(false);

        if (wallet == null)
            throw new InvalidOperationException("Can't found wallet.");

        var request = new WalletExchangeRequest
        {
            WalletId = command.WalletId,
            TransactionType = TransactionType.Withdrawal.ToString(),
            BrandId = wallet.BrandId,
            PlayerCode = wallet.PlayerCode,
            Amount = -command.Amount,
            Description = command.Description ?? string.Empty,
            ExternalId = command.ExternalId ?? string.Empty
        };

        return await _walletExchangeService.SendRequestAsync(request).ConfigureAwait(false);
    }
}
