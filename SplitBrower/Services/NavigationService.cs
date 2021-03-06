using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using GPS.SplitBrowser.Contracts.Services;
using GPS.SplitBrowser.Contracts.ViewModels;
using GPS.SplitBrowser.Helpers;

namespace GPS.SplitBrowser.Services
{
    // For more information on navigation between pages see
    // https://github.com/microsoft/TemplateStudio/blob/main/docs/WinUI/navigation.md
    public class NavigationService : INavigationService
    {
        private readonly IPageService _pageService;
        private object _lastParameterUsed;
        private Frame _frame;

        public event NavigatedEventHandler Navigated;

        public Frame Frame
        {
            get
            {
                if (_frame is null && App.Instance?.MainWindow?.Content is Frame f)
                {
                    _frame = f;
                    RegisterFrameEvents();
                }

#pragma warning disable CS8603 // Possible null reference return.
                return _frame;
#pragma warning restore CS8603 // Possible null reference return.
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        public bool CanGoBack => Frame.CanGoBack;

        public NavigationService(IPageService pageService)
            => _pageService = pageService;

        private void RegisterFrameEvents()
        {
            if (_frame is not null)
            {
                _frame.Navigated += OnNavigated;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (_frame is not null)
            {
                _frame.Navigated -= OnNavigated;
            }
        }

        public bool GoBack()
        {
            if (CanGoBack)
            {
                var vmBeforeNavigation = _frame.GetPageViewModel();
                _frame.GoBack();
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }

                return true;
            }

            return false;
        }

        public bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false)
        {
            var pageType = _pageService.GetPageType(pageKey);

            if (_frame.Content?.GetType() != pageType || (parameter is not null && !parameter.Equals(_lastParameterUsed)))
            {
                _frame.Tag = clearNavigation;
                var vmBeforeNavigation = _frame.GetPageViewModel();
                var navigated = _frame.Navigate(pageType, parameter);
                if (navigated)
                {
                    _lastParameterUsed = parameter;
                    if (vmBeforeNavigation is INavigationAware navigationAware)
                    {
                        navigationAware.OnNavigatedFrom();
                    }
                }

                return navigated;
            }

            return false;
        }

        public void CleanNavigation()
            => _frame.BackStack.Clear();

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                bool clearNavigation = (bool)frame.Tag;
                if (clearNavigation)
                {
                    frame.BackStack.Clear();
                }

                if (frame.GetPageViewModel() is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.Parameter);
                }

                Navigated?.Invoke(sender, e);
            }
        }
    }
}
