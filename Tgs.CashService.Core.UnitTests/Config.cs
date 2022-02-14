using System;
using NUnit.Framework;

namespace Tgs.CashService.Core.UnitTests;

[SetUpFixture]
public class Config
{
    [OneTimeSetUp]
    public void Setup()
    {
        _ = ThreadPool.SetMinThreads(Environment.ProcessorCount * 20, Environment.ProcessorCount);
    }
}
