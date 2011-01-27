﻿using System;

namespace OpenStory.Common
{
    /// <summary>
    /// Elemental attributes for a game skill.
    /// </summary>
    [Flags]
    public enum Elements
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,
        /// <summary>
        /// The fire attribute.
        /// </summary>
        Fire = 0x1, // F
        /// <summary>
        /// The ice attribute.
        /// </summary>
        Ice = 0x2, // I
        /// <summary>
        /// The lightning attribute.
        /// </summary>
        Ligtning = 0x4, // L
        /// <summary>
        /// The poison attribute.
        /// </summary>
        Poison = 0x8, // S
        /// <summary>
        /// The holy attribute.
        /// </summary>
        Holy = 0x10, // H
        /// <summary>
        /// The dark attribute.
        /// </summary>
        Dark = 0x20, // D
        /// <summary>
        /// No elemental attributes, a physical attack.
        /// </summary>
        Physical = 0x40, // P

        /// <summary>
        /// The fire and poison attributes.
        /// </summary>
        FirePoison = Fire | Poison,
        /// <summary>
        /// The ice and lightning attributes.
        /// </summary>
        IceLightning = Ice | Ligtning,
}
}