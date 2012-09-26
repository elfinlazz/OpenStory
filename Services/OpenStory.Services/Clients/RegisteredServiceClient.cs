﻿using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using OpenStory.Services.Contracts;

namespace OpenStory.Services.Clients
{
    /// <summary>
    /// Represents a client for a generic game service.
    /// </summary>
    /// <remarks>
    /// This class is abstract.
    /// </remarks>
    public sealed class RegisteredServiceClient : DuplexClientBase<IRegisteredService>, IRegisteredService
    {
        /// <summary>
        /// Initialized a new instance of <see cref="RegisteredServiceClient"/> with the specified endpoint address.
        /// </summary>
        /// <param name="endpoint">The service endpoint information.</param>
        /// <param name="stateChangedHandler">The handler for the service state changes.</param>
        public RegisteredServiceClient(ServiceEndpoint endpoint, IServiceStateChanged stateChangedHandler)
            : base(new InstanceContext(stateChangedHandler), endpoint)
        {
        }

        #region Implementation of IRegisteredService

        /// <inheritdoc />
        public ServiceOperationResult Initialize()
        {
            return ServiceOperationResult.Of(() => base.Channel.Initialize());
        }

        /// <inheritdoc />
        public ServiceOperationResult Start()
        {
            return ServiceOperationResult.Of(() => base.Channel.Start());
        }

        /// <inheritdoc />
        public ServiceOperationResult Stop()
        {
            return ServiceOperationResult.Of(() => base.Channel.Stop());
        }

        /// <inheritdoc />
        public ServiceOperationResult GetServiceState()
        {
            return ServiceOperationResult.Of(() => base.Channel.GetServiceState());
        }

        #endregion
    }
}