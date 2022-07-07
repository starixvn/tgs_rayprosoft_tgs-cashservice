namespace Tgs.CashService.ApplicationServices
{
    public record GameRevisionCommand(
        Guid WalletId,
        decimal Amount,
        string ExternalId);
}
