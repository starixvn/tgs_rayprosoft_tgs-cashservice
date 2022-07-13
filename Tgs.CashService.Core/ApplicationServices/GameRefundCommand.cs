namespace Tgs.CashService.ApplicationServices
{
    public record GameRefundCommand(
        Guid WalletId,
        decimal Amount,
        string ExternalId);
}
