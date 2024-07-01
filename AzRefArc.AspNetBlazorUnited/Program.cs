using AzRefArc.AspNetBlazorUnited.Client.Components;
using AzRefArc.AspNetBlazorUnited.Components;
using AzRefArc.AspNetBlazorUnited.Components.Pages.Samples;
using AzRefArc.AspNetBlazorUnited.Data;
using Microsoft.EntityFrameworkCore;
using AzRefArc.AspNetBlazorUnited.Client.Pages.Samples;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AzRefArc.AspNetBlazorUnited
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // per page, auto でプロジェクトを生成

            var builder = WebApplication.CreateBuilder(args);

            // Web API 利用のために追加
            builder.Services.AddControllers(); 
            
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            // 例外ログのファイル出力機能の追加
            builder.Logging.AddProvider(new ExceptionFileLoggerProvider());

            // DB サービス登録
            // AddDbContext() は Blazor Server では使ってはいけない (Scoped になる)、AddDbContextFactory() を使う
            // https://docs.microsoft.com/ja-jp/aspnet/core/blazor/blazor-server-ef-core
            builder.Services.AddDbContextFactory<PubsDbContext>(opt =>
            {
                // DbContext 構成設定
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

            // アプリサービスの追加(サーバ用)
            builder.Services.AddScoped(typeof(IInteractiveAutoListAuthorsService), typeof(InteractiveAutoListAuthorsServiceServerImpl));

            // 本番環境用の場合、サーバ間で鍵をそろえる必要がある
            // 鍵はファイル共有や Keyvault に格納できるが、ここでは追加リソースを構成する手間を避けるために DB に保存してしまうことにする
            // 本番環境用の DB に対して以下のクエリを実行し、テーブルを追加で作成しておく
            // CREATE TABLE[dbo].[DataProtectionKeys] ( [ID][int] IDENTITY(1, 1) NOT NULL PRIMARY KEY, [FriendlyName] [varchar] (64) NULL, [Xml][text] NULL)
            // ※ 開発環境ではこのテーブルが不要になるように、ここでは同一 DB に対して二つの DbContext を使うように実装している
            // この機能を使う場合には、DataProtection:UseSharedKeyOnDatabase 設定を true にすること
            //"DataProtection": {
            //    "UseSharedKeyOnDatabase":  true
            //}
            string? useSharedKeyOnDatabase = builder.Configuration["DataProtection:UseSharedKeyOnDatabase"];
            if (string.IsNullOrEmpty(useSharedKeyOnDatabase) == false && bool.Parse(useSharedKeyOnDatabase))
            {
                builder.Services.AddDbContextFactory<DataProtectionKeyDbContext>(opt =>
                {
                    // DbContext 構成設定
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
                builder.Services.AddDataProtection().PersistKeysToDbContext<DataProtectionKeyDbContext>();
            }

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

            // Web API 利用のために追加
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(AzRefArc.AspNetBlazorUnited.Client._Imports).Assembly);

            app.Run();
        }
    }
}
