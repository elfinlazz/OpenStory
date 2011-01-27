﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenStory.Cryptography
{
    /// <summary>
    /// Represents an unpacker for packets.
    /// </summary>
    public sealed class Unpacker
    {
        /// <summary>
        /// Gets the AES transform object for this Unpacker.
        /// </summary>
        public AesTransform AesTransform { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Unpacker class.
        /// </summary>
        /// <param name="iv">The IV for the internal AES transformation.</param>
        /// <param name="version">The game version.</param>
        public Unpacker(byte[] iv, short version)
        {
            this.AesTransform = new AesTransform(iv, version);
        }

        /// <summary>
        /// Decrypts raw packet data in-place.
        /// </summary>
        /// <param name="packetRawData">The raw data to decrypt.</param>
        public void Decrypt(byte[] packetRawData)
        {
            CustomEncryption.Decrypt(packetRawData);
            this.AesTransform.Transform(packetRawData);
        }

        /// <summary>
        /// Checks if an array is a valid packet header.
        /// </summary>
        /// <param name="header">The array to check.</param>
        /// <returns>true if the header is valid; otherwise, false.</returns>
        public bool CheckHeader(byte[] header)
        {
            return this.AesTransform.CheckHeader(header);
        }

        /// <summary>
        /// Gets the length of a packet from its header.
        /// </summary>
        /// <param name="header">The array to get the packet information from.</param>
        /// <returns>The length of the packet.</returns>
        public int GetLength(byte[] header)
        {
            return AesTransform.GetPacketLength(header);
        }
    }
}