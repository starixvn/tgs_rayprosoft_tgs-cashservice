namespace Tgs.CashService.ApplicationServices
{
    public record GameCreditCommand(
        Guid WalletId,
        decimal Amount,
        string ExternalId);
}
