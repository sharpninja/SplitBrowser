using Microsoft.UI.Xaml.Controls;

using GPS.SplitBrowser.ViewModels;

namespace GPS.SplitBrowser.Views
{
    // To learn more about WebView2, see https://docs.microsoft.com/microsoft-edge/webview2/
    public sealed partial class WebViewPage : Page
    {
        public WebViewViewModel ViewModel { get; }

        public WebViewPage()
        {
            ViewModel = App.Instance?.GetService<WebViewViewModel>()
                ?? throw new NullReferenceException(nameof(WebViewViewModel));
            InitializeComponent();
            ViewModel.WebViewService.Initialize(webView);
        }
    }
}
