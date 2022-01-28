using Google.Protobuf;
using Tgs.Cash.Protos;
using Tgs.CashService.Core;
using Tgs.CashService.Core.ApplicationServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.CashServiceWorker.MessageQueue;

public class RequestProcessor : IMessageProcessor
{
    private readonly IGetService<ExchangeWalletCommand, ExchangeResult> _exchangeService;

    public RequestProcessor(IGetService<ExchangeWalletCommand, ExchangeResult> exchangeService)
    {
        _exchangeService = exchangeService ?? throw new ArgumentNullException(nameof(exchangeService));
    }

    public async ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = WalletExchangeRequest.Parser.ParseFrom(data);

        var result = await _exchangeService.GetAsync(
            new ExchangeWalletCommand(
                request.WalletId,
                Enum.Parse<TransactionType>(request.TransactionType, true),
                request.BrandId,
                request.PlayerCode,
                request.Amount,
                request.Description,
                request.ExternalId,
                request.CreatedUser),
            cancellationToken).ConfigureAwait(false);

        var response = new WalletExchangeResponse
        {
            TransactionId = result.TransactionId,
            Balance = result.Balance
        };

        return response.ToByteArray();
    }
}
