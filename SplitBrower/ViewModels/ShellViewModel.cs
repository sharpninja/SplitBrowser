using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;

using GPS.SplitBrowser.Contracts.Services;

namespace GPS.SplitBrowser.ViewModels
{
    // You can show pages in different ways (update main view, navigate or right pane)
    // using the NavigationService and RightPaneService.
    // Read more about MenuBar project type here:
    // https://github.com/microsoft/TemplateStudio/blob/main/docs/WinUI/projectTypes/menubar.md
    public class ShellViewModel : ObservableRecipient
    {
        private bool _isBackEnabled;
        private object _selected;
        private ICommand _menuFileSettingsCommand;
        private ICommand _menuViewsWebViewCommand;
        private ICommand _menuFileExitCommand;

        public ICommand MenuFileExitCommand =>
            _menuFileExitCommand ??= new RelayCommand(OnMenuFileExit);

        public ICommand MenuFileSettingsCommand =>
            _menuFileSettingsCommand ??= new RelayCommand(OnMenuFileSettings);

        public ICommand MenuViewsWebViewCommand =>
            _menuViewsWebViewCommand ??= new RelayCommand(OnMenuViewsWebView);

        public INavigationService NavigationService { get; }

        public IRightPaneService RightPaneService { get; }

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { SetProperty(ref _isBackEnabled, value); }
        }

        public object Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ShellViewModel(INavigationService navigationService, IRightPaneService rightPaneService)
        {
            NavigationService = navigationService;
            NavigationService.Navigated += OnNavigated;
            RightPaneService = rightPaneService;
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
        }

        private void OnMenuFileExit() =>
            Application.Current.Exit();

        private void OnMenuViewsWebView() =>
            NavigationService.NavigateTo(typeof(WebViewViewModel).FullName);

        private void OnMenuFileSettings() =>
            RightPaneService.OpenInRightPane(typeof(SettingsViewModel).FullName);
    }
}
