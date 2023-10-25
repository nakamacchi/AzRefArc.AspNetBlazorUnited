using AzRefArc.AspNetBlazorUnited.Client;
using AzRefArc.AspNetBlazorUnited.Client.Pages.Samples;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// httpClient �T�[�r�X�̒ǉ�
string? baseAddress = builder.Configuration.GetValue<string>("BaseUrl");
if (baseAddress == null)
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
}
else
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
}

// �A�v���T�[�r�X�̒ǉ�(�N���C�A���g�p)
builder.Services.AddScoped(typeof(IInteractiveAutoListAuthorsService), typeof(InteractiveAutoListAuthorsServiceClientImpl));

// ��O���O�̃t�@�C���o�͋@�\�̒ǉ�
builder.Services.AddSingleton<ILoggerProvider, ExceptionFileLoggerProvider>();

await builder.Build().RunAsync();

