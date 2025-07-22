using AmazonSPAPIClient;
using Azure.Identity;
using CasioUKDeliveriesMVVM;
using ColoursOperations;
using DataServicesNET80.DatabaseAccessLayer;
using denEbayNET80;
using denLabelMVVM;
using denQuickbooksNET80;
using denQuickbooksNET80.Models;
using denSharedLibrary;
using denViewModels;
using denViewModels.LabelManager;
using denWPFSharedLibrary;
using EmailService;
using EntityEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OauthApi;
using Printers;
using Serilog;
using SettingsKeptInFile;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using ProBroViewModel = denViewModels.ProductBrowser.ProBro.ProBroViewModel;


namespace denMVVM;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static ServiceProvider? ServiceProvider;


    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush(); 
        base.OnExit(e);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
  
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/myapplog-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("Aplikacja wystartowała");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.amazon.json", optional: true, reloadOnChange: true)
            .Build();

        var vaultUri = new Uri(configuration["AzureKeyVault:VaultUri"]);
        configuration = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddAzureKeyVault(vaultUri, new DefaultAzureCredential())
            .Build();

        // --- KROK 2: Skonfiguruj usługi, przekazując im gotową konfigurację ---
        var serviceCollection = new ServiceCollection();
        // Przekaż obiekt 'configuration' do metody konfigurującej
        ConfigureServices(serviceCollection, configuration);
        ServiceProvider = serviceCollection.BuildServiceProvider();

        // --- KROK 3: Kontynuuj standardowe uruchamianie aplikacji ---
        base.OnStartup(e);

        CultureInfo cultureInfo = new("en-GB");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    public class GuiClientSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        
        services.Configure<AmazonApiSettings>(configuration.GetSection("Amazon"));
        services.Configure<QuickBooksSettings>(configuration.GetSection("QuickBooks"));
        services.Configure<GuiClientSettings>(configuration.GetSection("GuiClient"));
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.AddSingleton(configuration);

        services.AddHttpClient(EbayService.ServiceName, _ =>
        {

        });
        services.AddHttpClient(AmazonSpApi.ServiceName, _ =>
        {

        });
        services.AddHttpClient("NLOGokan", _ =>
        {

        });
        services.AddTransient<IEbayService, EbayService>();
        services.AddTransient<IMarketActions, MarketActions>();
        services.AddTransient<IOauthService, OauthService>();
        services.AddTransient<IQuickBooksService, QuickBooksService>();
        services.AddTransient<IAmazonSpApi, AmazonSpApi>();
        services.AddSingleton<IDatabaseAccessLayer, DatabaseAccessLayer>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>(provider => new MainWindow(provider));
        services.AddSingleton<IColourOpsMediator, WpfColourOpsMediator>();
        services.AddSingleton<ICurrentKomplety, CurrentKomplety>();
        services.AddSingleton<Dispatcher>(sp => Dispatcher.CurrentDispatcher);
        services.AddSingleton<IDispatcherService, WpfDispatcherService>();
        services.AddSingleton<ICompletesToXpsStream, CompletesToXpsStream>();
        services.AddSingleton<IFileDialogService, WpfFileDialogService>();
        services.AddSingleton<IXpsPrinter, XpsPrinter>();
        services.AddSingleton<IClipboardService, WpfClipboardService>();
        services.AddSingleton<IDispatcherTimerFactory, WpfDispatcherTimerFactory>();
        services.AddSingleton<IBoxLabelToImageByteArray, BoxLabelToImageByteArray>();
        services.AddSingleton<IOrdersSummaryToXpsStream, OrdersSummaryToXpsStream>();
        services.AddSingleton<IDpiService, WinApiDpiService>();
        services.AddSingleton<ITerminalScreenViewModel, TerminalScreenViewModel>();
        services.AddTransient<CurOrdViewModel>();
        services.AddTransient<TerminalScreenViewModel>();
        services.AddTransient<ProBroViewModel>();
        services.AddTransient<OrdBroViewModel>();
        services.AddTransient<WatchExplorerMVVM.WatchExplorerViewModel>();
        services.AddTransient<StorageExplorerMVVM.StorageExplorerViewModel>();
        services.AddTransient<ColourBrowserMVVM.ColourBrowserViewModel>();
        services.AddTransient<MayAlsoFitMVVM.MayAlsoFitViewModel>();
        services.AddTransient<CasioUKDeliveriesViewModel>();
        services.AddTransient<TypesMVVM.TypesViewModel>();
        services.AddTransient<PrametersViewModel>();
        services.AddTransient<PngAnimatorControlViewModel>();
        services.AddTransient<LabelControlViewModel>();
        services.AddSingleton<ViewModelFactory>();
        services.AddTransient<BothLabelsContainerViewModel>();
        services.AddTransient<LabelControlViewModel>();
        services.AddTransient<Cn22SettingsViewModel>();
        services.AddTransient<CN22SettingsControl>();
        services.AddTransient<LabelControl>();
        services.AddTransient<BusinessDetailsViewModel>();
        services.AddTransient<SalesSummaryViewModel>();
        services.AddSingleton<IEmailService, EmailService.EmailService>();
        services.AddSingleton<IEntityEventsService, EntityEvents.EntityEventsService>();
        services.AddSingleton<ISettingsService>(_ => new SettingsService("cfg\\settings.cfg"));
        services.AddSingleton<IPrintersService, PrintersService>();
        services.AddSingleton<IColoursService, ColoursService>();
    }
}