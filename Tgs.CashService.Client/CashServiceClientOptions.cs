using System;
using System.Text.Json;

namespace Tgs.CashService.Client;

public class CashServiceClientOptions
{
    public Uri BaseUrl { get; set; } = default!;

    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
}
