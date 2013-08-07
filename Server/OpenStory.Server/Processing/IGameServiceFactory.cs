﻿using OpenStory.Services;

namespace OpenStory.Server.Processing
{
    /// <summary>
    /// Provides methods for creating <see cref="GameServiceBase"/> instances.
    /// </summary>
    public interface IGameServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="GameServiceBase"/> instance.
        /// </summary>
        GameServiceBase CreateService();
    }
}
