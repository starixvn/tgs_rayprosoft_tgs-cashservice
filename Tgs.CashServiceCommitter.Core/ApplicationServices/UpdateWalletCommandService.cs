using Tgs.ServiceUtility;

namespace Tgs.CashServiceCommitter.ApplicationServices;

internal class UpdateWalletCommandService(ITgsRepository tgsRepository) : ICommandService<UpdateWalletCommand>
{
    public ValueTask ExecuteAsync(UpdateWalletCommand command, CancellationToken cancellationToken = default)
        => tgsRepository.UpdateWalletAsync(command.WalletId, cancellationToken);
}
