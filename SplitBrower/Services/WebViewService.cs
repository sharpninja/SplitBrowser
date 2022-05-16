using System;

using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

using GPS.SplitBrowser.Contracts.Services;

namespace GPS.SplitBrowser.Services
{
    public class WebViewService : IWebViewService
    {
        private WebView2 _webView;

        public bool CanGoBack
            => _webView.CanGoBack;

        public bool CanGoForward
            => _webView.CanGoForward;

        public event EventHandler<CoreWebView2WebErrorStatus> NavigationCompleted;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public WebViewService()
        {
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(WebView2 webView)
        {
            _webView = webView;
            _webView.NavigationCompleted += OnWebViewNavigationCompleted;
        }

        public void UnregisterEvents()
        {
            _webView.NavigationCompleted -= OnWebViewNavigationCompleted;
        }

        private void OnWebViewNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            NavigationCompleted?.Invoke(this, args.WebErrorStatus);
        }

        public void GoBack()
            => _webView.GoBack();

        public void GoForward()
            => _webView.GoForward();

        public void Reload()
            => _webView.Reload();
    }
}
