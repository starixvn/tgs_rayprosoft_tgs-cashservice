using NUnit.Framework;

namespace Tgs.CashService.Core.UnitTests;

[SetUpFixture]
public class Config
{
    [OneTimeSetUp]
    public void Setup()
    {
        _ = ThreadPool.SetMinThreads(512, 10);
    }
}
