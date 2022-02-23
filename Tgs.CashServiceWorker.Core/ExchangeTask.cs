using Tgs.CashService;

namespace Tgs.CashServiceWorker;

internal record struct ExchangeTask(ExchangeRequest Request, TaskCompletionSource<ExchangeResult> Promise);
