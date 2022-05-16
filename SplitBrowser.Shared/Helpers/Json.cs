using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace GPS.SplitBrowser.Core.Helpers
{
    public static class Json
    {
        public static Task<T> ToObjectAsync<T>(string value)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return Task.Run<T>(() => JsonConvert.DeserializeObject<T>(value));
#pragma warning restore CS8603 // Possible null reference return.
        }

        public static async Task<string> StringifyAsync(object value)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(value));
        }
    }
}
