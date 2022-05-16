using System;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;

using GPS.SplitBrowser.Contracts.Services;

namespace GPS.SplitBrowser.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public ElementTheme Theme { get; set; } = ElementTheme.Default;

        private readonly ILocalSettingsService _localSettingsService;

        public ThemeSelectorService(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;
        }

        public async Task InitializeAsync()
        {
            Theme = await LoadThemeFromSettingsAsync();
        }

        public async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;

            await SetRequestedThemeAsync();
            await SaveThemeInSettingsAsync(Theme);
        }

        public Task SetRequestedThemeAsync()
        {
            if (App.Instance?.MainWindow?.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Theme;
            }

            return Task.CompletedTask;
        }

        private async Task<ElementTheme> LoadThemeFromSettingsAsync()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            string themeName = await _localSettingsService.ReadSettingAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        private Task SaveThemeInSettingsAsync(ElementTheme theme)
        {
            return _localSettingsService.SaveSettingAsync(SettingsKey, theme.ToString());
        }
    }
}
