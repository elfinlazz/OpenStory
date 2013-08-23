﻿using System.ServiceModel;
using OpenStory.Services;
using OpenStory.Services.Contracts;

namespace OpenStory.Server.Auth
{
    /// <summary>
    /// Represents a WCF service that hosts the Authentication Server instance.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class AuthService : GameServiceBase, IAuthService
    {
        /// <inheritdoc />
        public AuthService()
        {
        }
    }
}
