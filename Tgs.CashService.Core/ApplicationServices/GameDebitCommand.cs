namespace Tgs.CashService.ApplicationServices
{
    public record GameDebitCommand(
        Guid WalletId,
        decimal Amount,
        string ExternalId);
}
