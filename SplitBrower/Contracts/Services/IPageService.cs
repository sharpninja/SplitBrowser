using System;

namespace GPS.SplitBrowser.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
