namespace Tgs.CashService.Core;

internal record struct ExchangeTask(ExchangeRequest Request, TaskCompletionSource<ExchangeResult> Promise);
