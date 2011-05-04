﻿using System;
using OpenStory.Common.IO;

namespace OpenStory.Networking
{
    /// <summary>
    /// Incoming packet data, EventArgs-style.
    /// </summary>
    public class IncomingPacketEventArgs : EventArgs
    {
        private byte[] buffer;

        /// <summary>
        /// Initializes a new instance of the IncomingPacketEventArgs class.
        /// </summary>
        /// <param name="packet">The packet data.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="packet" /> is <c>null</c>.
        /// </exception>
        public IncomingPacketEventArgs(byte[] packet)
        {
            if (packet == null) throw new ArgumentNullException("packet");

            this.buffer = packet;
        }

        /// <summary>
        /// Gets a new <see cref="PacketReader"/> for the packet.
        /// </summary>
        public PacketReader Reader
        {
            get { return new PacketReader(buffer); }
        }
    }
}