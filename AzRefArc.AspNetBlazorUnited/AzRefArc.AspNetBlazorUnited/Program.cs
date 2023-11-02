using AzRefArc.AspNetBlazorUnited.Client.Components;
using AzRefArc.AspNetBlazorUnited.Components;
using AzRefArc.AspNetBlazorUnited.Components.Pages.Samples;
using AzRefArc.AspNetBlazorUnited.Data;
using Microsoft.EntityFrameworkCore;
using AzRefArc.AspNetBlazorUnited.Client.Pages.Samples;

namespace AzRefArc.AspNetBlazorUnited
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // per page, auto �Ńv���W�F�N�g�𐶐�

            var builder = WebApplication.CreateBuilder(args);

            // Web API ���p�̂��߂ɒǉ�
            builder.Services.AddControllers(); 
            
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            // ��O���O�̃t�@�C���o�͋@�\�̒ǉ�
            builder.Logging.AddProvider(new ExceptionFileLoggerProvider());

            // DB �T�[�r�X�o�^
            // AddDbContext() �� Blazor Server �ł͎g���Ă͂����Ȃ� (Scoped �ɂȂ�)�AAddDbContextFactory() ���g��
            // https://docs.microsoft.com/ja-jp/aspnet/core/blazor/blazor-server-ef-core
            builder.Services.AddDbContextFactory<PubsDbContext>(opt =>
            {
                // DbContext �\���ݒ�
                // https://docs.microsoft.com/ja-jp/ef/core/dbcontext-configuration/#other-dbcontext-configuration
                if (builder.Environment.IsDevelopment())
                {
                    opt = opt.EnableSensitiveDataLogging().EnableDetailedErrors();
                }
                opt.UseSqlServer(
                    builder.Configuration.GetConnectionString("PubsDbContext"),
                    providerOptions =>
                    {
                        providerOptions.EnableRetryOnFailure();
                    });
            });

            // �A�v���T�[�r�X�̒ǉ�(�T�[�o�p)
            builder.Services.AddScoped(typeof(IInteractiveAutoListAuthorsService), typeof(InteractiveAutoListAuthorsServiceServerImpl));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            // Web API ���p�̂��߂ɒǉ�
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(AzRefArc.AspNetBlazorUnited.Client._Imports).Assembly);

            app.Run();
        }
    }
}
