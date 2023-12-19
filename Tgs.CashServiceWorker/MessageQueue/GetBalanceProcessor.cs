using Google.Protobuf;
using Tgs.Cash.Protos;
using Tgs.CashServiceWorker.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.CashServiceWorker.MessageQueue;

public class GetBalanceProcessor(IGetService<GetBalanceQuery, decimal> getBalanceService)
    : IMessageProcessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>
{
    private readonly IGetService<GetBalanceQuery, decimal> _getBalanceService = getBalanceService
        ?? throw new ArgumentNullException(nameof(getBalanceService));

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = BalanceRequest.Parser.ParseFrom(data.Span);

        var response = new BalanceResponse
        {
            Balance = await _getBalanceService.GetAsync(
                new GetBalanceQuery(request.WalletId),
                cancellationToken).ConfigureAwait(false)
        };

        return response.ToByteArray();
    }
}
