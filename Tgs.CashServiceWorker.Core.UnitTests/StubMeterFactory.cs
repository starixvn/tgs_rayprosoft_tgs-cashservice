using System.Diagnostics.Metrics;

namespace Tgs.CashServiceWorker.Core.UnitTests;

internal class StubMeterFactory : IMeterFactory
{
    public Meter Create(MeterOptions options) => new Meter(options);

    public void Dispose()
    {
    }
}
