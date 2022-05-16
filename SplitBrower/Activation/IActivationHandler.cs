using System.Threading.Tasks;

namespace GPS.SplitBrowser.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);

        Task HandleAsync(object args);
    }
}
