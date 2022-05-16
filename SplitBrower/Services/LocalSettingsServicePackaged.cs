using System.Threading.Tasks;

using GPS.SplitBrowser.Contracts.Services;
using GPS.SplitBrowser.Core.Helpers;

using Windows.Storage;

namespace GPS.SplitBrowser.Services
{
    public class LocalSettingsServicePackaged : ILocalSettingsService
    {
        public async Task<T> ReadSettingAsync<T>(string key)
        {
            object? obj = null;

            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }

#pragma warning disable CS8603 // Possible null reference return.
            return default;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task SaveSettingAsync<T>(string key, T value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
        }
    }
}
