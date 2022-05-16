using GPS.SplitBrowser;

namespace GPS.SplitBrowser;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        var builder = new WindowsAppSdkHostBuilder<GPS.SplitBrowser.App>();

        builder.ConfigureServices((context, services) =>
         {
             // Window
             services.AddSingleton(_ => new Window()
             {
                 Title = "AppDisplayName".GetLocalized()
             });

             // Default Activation Handler
             services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

             // Other Activation Handlers

             // Services
             services.AddSingleton<ILocalSettingsService, LocalSettingsServicePackaged>();
             services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
             services.AddTransient<IWebViewService, WebViewService>();
             services.AddSingleton<IRightPaneService, RightPaneService>();

             services.AddSingleton<IActivationService, ActivationService>();
             services.AddSingleton<IPageService, PageService>();
             services.AddSingleton<INavigationService, NavigationService>();

             // Core Services
             services.AddSingleton<IFileService, FileService>();

             // Views and ViewModels
             services.AddTransient<SettingsViewModel>();
             services.AddTransient<SettingsPage>();
             services.AddTransient<WebViewViewModel>();
             services.AddTransient<WebViewPage>();
             services.AddTransient<ShellPage>();
             services.AddTransient<ShellViewModel>();

             // Configuration
             services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
         });

        var appHost = builder.Build();

        App.Instance = appHost.Services.GetRequiredService<GPS.SplitBrowser.App>();
        App.Instance.AppHost = appHost;

        appHost.StartAsync().GetAwaiter().GetResult();
    }

    private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        throw new NotImplementedException();
    }
}
