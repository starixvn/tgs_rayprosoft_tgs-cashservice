using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tgs.CashService.IntegrationTests;

internal class TestCashApplication : WebApplicationFactory<Program>
{
    private readonly Action<IWebHostBuilder>? _webHostConfigure;

    public TestCashApplication(Action<IWebHostBuilder>? webHostConfigure = null)
    {
        _webHostConfigure = webHostConfigure;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => _webHostConfigure?.Invoke(builder);
}
