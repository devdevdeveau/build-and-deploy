using BuildAndDeploy.Website;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<EchoService>()
    .AddHttpClient<EchoApi>(client =>
        client.BaseAddress =
            new Uri(builder.Configuration["EchoUri"] ?? throw new InvalidOperationException("Missing Configuration EchoUri")));

await builder.Build().RunAsync();

internal class EchoApi
{
    private readonly HttpClient _client;

    public EchoApi(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> PostPayload(string payload)
    {
        var result = await _client.PostAsync("/echo", new StringContent(payload));
        return await result.Content.ReadAsStringAsync();
    }
}

internal class EchoModel
{
    public string Value { get; set; }
    public string Result { get; set; }

    public EchoModel()
    {
        Value = string.Empty;
        Result = string.Empty;
    }
}

internal class EchoService
{
    private readonly EchoApi _api;

    public EchoService(EchoApi api)
    {
        _api = api;
    }

    public async Task<string> Echo(string value)
    {
        return await _api.PostPayload(value);
    }
}

