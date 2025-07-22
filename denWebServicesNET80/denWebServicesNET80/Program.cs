using AmazonSPAPIClient;
using Azure.Identity;
using DataServicesNET80;
using denEbayNET80;
using denQuickbooksNET80;
using denQuickbooksNET80.Models;
using denSharedLibrary;
using denWebServicesNET80.Models;
using denWebServicesNET80.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OauthApi;
using Serilog;
using System.Configuration;

namespace denWebServicesNET80;

internal class Program
{
    private static void Main(string[] args)
    {

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true);

        // 2. Użyj sekretów do połączenia z Azure Key Vault
        var vaultUri = new Uri(builder.Configuration["AzureKeyVault:VaultUri"]);
        var tenantId = builder.Configuration["AzureKeyVault:TenantId"];
        var clientId = builder.Configuration["AzureKeyVault:ClientId"];
        var clientSecret = builder.Configuration["AzureKeyVault:ClientSecret"];

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        builder.Configuration.AddAzureKeyVault(vaultUri, credential);


        builder.Host.UseSerilog((_, lc) => lc
            .MinimumLevel.Debug() 
            .WriteTo.Console()
            .WriteTo.File(
                "logs/myapp.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30, 
                fileSizeLimitBytes: 10485760,
                rollOnFileSizeLimit: true, 
                shared: true 
            )
        );
        ConfigureServices(builder);
        var app = builder.Build();
        ConfigurePipeline(app);
        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
        });

        builder.Services.AddAuthorizationBuilder();
        builder.Services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));

        builder.Services.AddDbContext<UsersAndClientsDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("UserRelatedConnection")));
        builder.Services.AddIdentityCore<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddApiEndpoints();
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 2147483648;
        });



        builder.Services.AddControllersWithViews();

        var dbSettings = builder.Configuration.GetSection("DatabaseSettings");

        DbContextFactory.SetDatabaseConfiguration(
            dbSettings["Host"],
            dbSettings["Port"],
            dbSettings["User"],
            dbSettings["Password"],
            dbSettings["DatabaseName"]
        );
        //var qbSettingsForTest = builder.Configuration.GetSection("QuickBooks").Get<QuickBooksSettings>();

        builder.Services.AddSingleton<DatabaseAccessLayerForConsoleFactory>();
        builder.Services.AddSingleton(provider =>
            provider.GetRequiredService<DatabaseAccessLayerForConsoleFactory>().Create());
        builder.Services.AddHttpClient(EbayService.ServiceName, _ => { /*...*/ });
        builder.Services.AddHttpClient(QuickBooksService.ServiceName, client => { /*...*/ });
        builder.Services.AddHttpClient(AmazonSpApi.ServiceName, client => { /*...*/ });
        builder.Services.AddScoped<IEbayService, EbayService>();
        builder.Services.AddScoped<IMarketActions, MarketActions>();
        builder.Services.AddTransient<IOauthService, OauthService>();
        builder.Services.AddScoped<IQuickBooksService, QuickBooksService>();
        builder.Services.AddScoped<IAmazonSpApi, AmazonSpApi>();
        builder.Services.AddScoped<IUserServices, UserServices>();
        builder.Services.AddSingleton<ConnectionCheckerService>();

        builder.Services.AddSignalR();

        builder.Services.AddTransient<ISignalRActions, SignalRActions>();
        builder.Services.Configure<AmazonApiSettings>(builder.Configuration.GetSection("AmazonApiSettings"));
        builder.Services.Configure<QuickBooksSettings>(builder.Configuration.GetSection("QuickBooks"));
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        app.MapIdentityApi<IdentityUser>();
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.MapHub<denHub>("denHub");
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();



        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }

    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : IdentityDbContext<IdentityUser>(options);
}