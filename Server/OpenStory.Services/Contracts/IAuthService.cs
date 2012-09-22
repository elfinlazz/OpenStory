using System.ServiceModel;

namespace OpenStory.Services.Contracts
{
    /// <summary>
    /// Provides methods for accessing and managing the Authentication Service.
    /// </summary>
    [ServiceContract(Namespace = null, Name = "AuthenticationService", CallbackContract = typeof(IServiceStateChanged))]
    public interface IAuthService : IManagedService
    {
    }
}