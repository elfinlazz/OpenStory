﻿using System;
using OpenStory.Common.Game;

namespace OpenStory.Common.IO
{
    /// <summary>
    /// Extension methods for packet IO.
    /// </summary>
    public static class IoExtensions
    {
        // You may use these as samples for writing custom readers/writers. 
        // Just write an extension method to PacketReader or PacketBuilder (or one of their interfaces like below) and it works!

        /// <summary>
        /// Reads a <see cref="PointS"/>.
        /// </summary>
        /// <param name="reader">The <see cref="IUnsafePacketReader">packet reader</see> to use.</param>
        /// <returns>a <see cref="PointS"/> that was read.</returns>
        public static PointS ReadVector(this IUnsafePacketReader reader)
        {
            var x = reader.ReadInt16();
            var y = reader.ReadInt16();
            return new PointS(x, y);
        }

        /// <summary>
        /// Writes a <see cref="PointS"/>.
        /// </summary>
        /// <param name="builder">The <see cref="PacketBuilder">packet builder</see> to use.</param>
        /// <param name="vector">The <see cref="PointS"/> to write.</param>
        public static void WriteVector(this IPacketBuilder builder, PointS vector)
        {
            builder.WriteInt16(vector.X);
            builder.WriteInt16(vector.Y);
        }
    }
}