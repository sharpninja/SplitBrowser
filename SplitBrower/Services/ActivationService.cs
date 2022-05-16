using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using GPS.SplitBrowser.Activation;
using GPS.SplitBrowser.Contracts.Services;
using GPS.SplitBrowser.Views;

namespace GPS.SplitBrowser.Services
{
    public class ActivationService : IActivationService
    {
        private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private readonly INavigationService _navigationService;
        private readonly IThemeSelectorService _themeSelectorService;
        private UIElement? _shell = null;

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService, IThemeSelectorService themeSelectorService)
        {
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
            _navigationService = navigationService;
            _themeSelectorService = themeSelectorService;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            // Initialize services that you need before app activation
            // take into account that the splash screen is shown while this code runs.
            await InitializeAsync();

            if (App.Instance?.MainWindow is not null){
                if (App.Instance.MainWindow.Content == null)
                {
                    _shell = App.Instance.GetRequiredService<ShellPage>();
                    App.Instance.MainWindow.Content = _shell ?? new Frame();
                }
            }
            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);

            // Ensure the current window is active
            App.Instance?.MainWindow?.Activate();

            // Tasks after activation
            await StartupAsync();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = _activationHandlers
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler is not null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (_defaultHandler.CanHandle(activationArgs))
            {
                await _defaultHandler.HandleAsync(activationArgs);
            }
        }

        private Task InitializeAsync()
        {
            return _themeSelectorService.InitializeAsync();
        }

        private Task StartupAsync()
        {
            return _themeSelectorService.SetRequestedThemeAsync();
        }
    }
}
