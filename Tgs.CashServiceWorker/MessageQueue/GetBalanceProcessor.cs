using Google.Protobuf;
using Tgs.Cash.Protos;
using Tgs.CashService.Core;
using Tgs.MessageQueue;

namespace Tgs.CashServiceWorker.MessageQueue;

public class GetBalanceProcessor : IMessageProcessor
{
    private readonly IWalletRepository _walletRepository;

    public GetBalanceProcessor(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
    }

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = BalanceRequest.Parser.ParseFrom(data.Span);

        var balance = await _walletRepository
            .GetBalanceAsync(request.WalletId)
            .ConfigureAwait(false);

        var response = new BalanceResponse
        {
            Balance = balance is null ? throw new InvalidOperationException("Can't found wallet.") : balance.Value
        };

        return response.ToByteArray();
    }
}
