namespace Tgs.CashService;

public class WalletExchangeFailException : Exception
{
    public WalletExchangeFailException()
    {
    }

    public WalletExchangeFailException(string message): base(message)
    {
    }
}
