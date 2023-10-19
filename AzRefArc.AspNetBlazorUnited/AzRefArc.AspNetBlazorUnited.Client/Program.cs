using AzRefArc.AspNetBlazorUnited.Client.Pages.Samples;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// httpClient サービスの追加
string? baseAddress = builder.Configuration.GetValue<string>("BaseUrl");
if (baseAddress == null)
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
}
else
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
}

// アプリサービスの追加(クライアント用)
builder.Services.AddScoped(typeof(IInteractiveAutoListAuthorsService), typeof(InteractiveAutoListAuthorsServiceClientImpl));

await builder.Build().RunAsync();

