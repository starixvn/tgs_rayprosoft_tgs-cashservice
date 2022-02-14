using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tgs.CashService.IntegrationTests;

[TestClass]
public class Config
{
    [AssemblyInitialize]
#pragma warning disable IDE0060 // 移除未使用的參數
    public static void Setup(TestContext context)
#pragma warning restore IDE0060 // 移除未使用的參數
    {
        _ = ThreadPool.SetMinThreads(Environment.ProcessorCount * 20, Environment.ProcessorCount);
    }
}
