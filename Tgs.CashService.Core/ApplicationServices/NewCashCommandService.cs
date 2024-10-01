using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class NewCashCommandService(
    ITgsRepository tgsRepository)
    : IGetService<NewCashCommand, Guid>
{
    public async ValueTask<Guid> GetAsync(NewCashCommand command, CancellationToken cancellationToken = default)
    {
        var walletId = Guid.NewGuid();

        await tgsRepository.CreateWalletAsync(
            walletId,
            command.BrandId,
            command.UserId,
            command.Type,
            command.CurrencyCode,
            cancellationToken).ConfigureAwait(false);

        return walletId;
    }
}
