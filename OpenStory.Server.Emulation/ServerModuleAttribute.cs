﻿using System;

namespace OpenStory.Server.Emulation
{
    /// <summary>
    /// Indicates that the class is a server module which needs initialization.
    /// </summary>
    /// <remarks>
    /// The initialization routines will call methods marked by the
    /// <see cref="InitializationMethodAttribute"/> in the class.
    /// Note, the initialization order of separate modules 
    /// within the same <see cref="InitializationStage"/> is undefined.
    /// The execution order of initialization methods within a server 
    /// module class is also undefined.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ServerModuleAttribute : Attribute
    {
        private readonly InitializationStage stage;

        /// <summary>
        /// Specifies that the class is a server module and should be initialized.
        /// </summary>
        /// <param name="initializationStage">The initialization stage for the server module.</param>
        public ServerModuleAttribute(InitializationStage initializationStage)
        {
            this.stage = initializationStage;
        }

        /// <summary>
        /// Gets the initialization stage for the server module
        /// </summary>
        public InitializationStage InitializationStage
        {
            get { return this.stage; }
        }
    }
}
