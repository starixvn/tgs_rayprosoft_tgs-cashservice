namespace Tgs.CashServiceWorker;

public class Wallet
{
    public Guid Id { get; set; }

    public Guid BrandId { get; set; }

    public string PlayerCode { get; set; } = default!;
}
