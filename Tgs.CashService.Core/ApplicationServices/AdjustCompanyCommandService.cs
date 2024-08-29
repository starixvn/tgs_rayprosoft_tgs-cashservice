using System.Text.Json;
using Tgs.CashService.Models;
using Tgs.ServiceUtility;

namespace Tgs.CashService.ApplicationServices;

internal class AdjustCompanyCommandService(ITgsRepository tgsRepository)
    : IGetService<AdjustCompanyV2Command, ExchangeResult>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public ValueTask<ExchangeResult> GetAsync(AdjustCompanyV2Command command, CancellationToken cancellationToken = default)
    {
        var description = new
        {
            TransactionType = (int)TransactionType.AdjustCompany,
            Remark = command.Description
        };
        var record = new ExchangeWalletRecord(
            command.WalletId,
            TransactionType.AdjustCompany,
            command.Amount,
            0,
            JsonSerializer.Serialize(description),
            command.ExternalId ?? string.Empty,
            command.CreatedByUserId
        );
        return _tgsRepository.ExchangeWalletAsync(record, cancellationToken);
    }
}
