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

            // �{�Ԋ��p�̏ꍇ�A�T�[�o�ԂŌ������낦��K�v������
            // ���̓t�@�C�����L�� Keyvault �Ɋi�[�ł��邪�A�����ł͒ǉ����\�[�X���\�������Ԃ�����邽�߂� DB �ɕۑ����Ă��܂����Ƃɂ���
            // �{�Ԋ��p�� DB �ɑ΂��Ĉȉ��̃N�G�������s���A�e�[�u����ǉ��ō쐬���Ă���
            // CREATE TABLE[dbo].[DataProtectionKeys] ( [ID][int] IDENTITY(1, 1) NOT NULL PRIMARY KEY, [FriendlyName] [varchar] (64) NULL, [Xml][text] NULL)
            // �� �J�����ł͂��̃e�[�u�����s�v�ɂȂ�悤�ɁA�����ł͓��� DB �ɑ΂��ē�� DbContext ���g���悤�Ɏ������Ă���
            // ���̋@�\���g���ꍇ�ɂ́ADataProtection:UseSharedKeyOnDatabase �ݒ�� true �ɂ��邱��
            //"DataProtection": {
            //    "UseSharedKeyOnDatabase":  true
            //}
            string? useSharedKeyOnDatabase = builder.Configuration["DataProtection:UseSharedKeyOnDatabase"];
            if (string.IsNullOrEmpty(useSharedKeyOnDatabase) == false && bool.Parse(useSharedKeyOnDatabase))
            {
                builder.Services.AddDbContextFactory<DataProtectionKeyDbContext>(opt =>
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
                builder.Services.AddDataProtection().PersistKeysToDbContext<DataProtectionKeyDbContext>().SetApplicationName("AzRefArc.AspNetBlazorUnited");
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
