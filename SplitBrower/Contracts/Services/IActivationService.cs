using System.Threading.Tasks;

namespace GPS.SplitBrowser.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
