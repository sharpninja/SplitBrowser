#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning disable MVVMTK0001 // Duplicate INotifyPropertyChanged definition
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Web.WebView2.Core;

using GPS.SplitBrowser.Contracts.Services;
using GPS.SplitBrowser.Contracts.ViewModels;

namespace GPS.SplitBrowser.ViewModels;

// TODO: Review best practices and distribution guidelines for apps using WebView2.
// https://docs.microsoft.com/microsoft-edge/webview2/concepts/developer-guide
// https://docs.microsoft.com/microsoft-edge/webview2/concepts/distribution
//
// You can also read more about WebView2 control at
// https://docs.microsoft.com/microsoft-edge/webview2/get-started/winui.
[INotifyPropertyChanged]
public partial class WebViewViewModel : ObservableRecipient, INavigationAware
{
    // TODO: Set the default URL to display.
    private const string DefaultUrl = "https://docs.microsoft.com/windows/apps/";

    [ObservableProperty]
    private Uri _source;
    [ObservableProperty]
    private bool _isLoading = true;
    [ObservableProperty]
    private bool _hasFailures;

    private ICommand _browserBackCommand;
    private ICommand _browserForwardCommand;
    private ICommand _openInBrowserCommand;
    private ICommand _reloadCommand;
    private ICommand _retryCommand;

    public IWebViewService WebViewService { get; }

    public ICommand BrowserBackCommand =>
        _browserBackCommand ??= new RelayCommand(
        () => WebViewService?.GoBack(), () => WebViewService?.CanGoBack ?? false);

    public ICommand BrowserForwardCommand =>
        _browserForwardCommand ??= new RelayCommand(
        () => WebViewService?.GoForward(), () => WebViewService?.CanGoForward ?? false);

    public ICommand ReloadCommand =>
        _reloadCommand ??= new RelayCommand(
        () => WebViewService?.Reload());

    public ICommand RetryCommand =>
        _retryCommand ??= new RelayCommand(OnRetry);

    public ICommand OpenInBrowserCommand
    {
        get
        {
            async void execute() => await Windows.System.Launcher.LaunchUriAsync(Source);
            return _openInBrowserCommand ??= new RelayCommand(execute);
        }
    }

    public WebViewViewModel(IWebViewService webViewService)
        => WebViewService = webViewService;

    public void OnNavigatedTo(object parameter)
    {
        WebViewService.NavigationCompleted += OnNavigationCompleted;
        Source = new Uri(DefaultUrl);
    }

    public void OnNavigatedFrom()
    {
        WebViewService.UnregisterEvents();
        WebViewService.NavigationCompleted -= OnNavigationCompleted;
    }

    private void OnNavigationCompleted(object sender, CoreWebView2WebErrorStatus webErrorStatus)
    {
        IsLoading = false;
        OnPropertyChanged(nameof(BrowserBackCommand));
        OnPropertyChanged(nameof(BrowserForwardCommand));
        if (webErrorStatus != default)
        {
            // Use `webErrorStatus` to vary the displayed message based on the error reason
            HasFailures = true;
        }
    }

    private void OnRetry()
    {
        HasFailures = false;
        IsLoading = true;
        WebViewService?.Reload();
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore MVVMTK0001 // Duplicate INotifyPropertyChanged definition
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
