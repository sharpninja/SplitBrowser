﻿using System.Threading.Tasks;

using Microsoft.UI.Xaml;

namespace GPS.SplitBrowser.Contracts.Services
{
    public interface IThemeSelectorService
    {
        ElementTheme Theme { get; }

        Task InitializeAsync();

        Task SetThemeAsync(ElementTheme theme);

        Task SetRequestedThemeAsync();
    }
}
