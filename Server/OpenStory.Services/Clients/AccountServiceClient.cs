﻿using System;
using System.ServiceModel;
using OpenStory.Services.Contracts;

namespace OpenStory.Services.Clients
{
    /// <summary>
    /// Represents a client for a game account service.
    /// </summary>
    public sealed class AccountServiceClient : ManagedServiceClient<IAccountService>, IAccountService
    {
        /// <summary>
        /// Initialized a new instance of <see cref="AccountServiceClient"/> with the specified endpoint address.
        /// </summary>
        /// <param name="uri">The URI of the service to connect to.</param>
        public AccountServiceClient(Uri uri)
            : base(uri)
        {
        }

        #region IAccountService Members

        /// <inheritdoc />
        public bool TryRegisterSession(int accountId, out int sessionId)
        {
            return base.Channel.TryRegisterSession(accountId, out sessionId);
        }

        /// <inheritdoc />
        public bool TryRegisterCharacter(int accountId, int characterId)
        {
            return base.Channel.TryRegisterCharacter(accountId, characterId);
        }

        /// <inheritdoc />
        public bool TryUnregisterSession(int accountId)
        {
            return base.Channel.TryUnregisterSession(accountId);
        }

        /// <inheritdoc />
        public bool TryKeepAlive(int accountId, out TimeSpan lag)
        {
            try
            {
                return base.Channel.TryKeepAlive(accountId, out lag);
            }
            catch (EndpointNotFoundException)
            {
                lag = default(TimeSpan);
                return false;
            }
            catch (TimeoutException)
            {
                lag = default(TimeSpan);
                return false;
            }
        }

        #endregion
    }
}