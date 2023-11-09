using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class NewCashCommandService : IGetService<NewCashCommand, Guid>
{
    private readonly ITgsRepository _tgsRepository;

    public NewCashCommandService(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

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

        return walletId;
    }
}
