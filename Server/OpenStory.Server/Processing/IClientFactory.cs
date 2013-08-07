using OpenStory.Framework.Contracts;

namespace OpenStory.Server.Processing
{
    /// <summary>
    /// Provides methods for creating client instances.
    /// </summary>
    /// <typeparam name="TClient">The type of the clients.</typeparam>
    public interface IClientFactory<out TClient>
        where TClient : ClientBase
    {
        /// <summary>
        /// Creates a new client instance.
        /// </summary>
        /// <param name="session">The underlying session for the new client.</param>
        /// <returns>the new <typeparamref name="TClient"/> instance.</returns>
        TClient CreateClient(IServerSession session);
    }
}