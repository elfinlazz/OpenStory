﻿using System;

namespace OpenStory.Common.IO
{
    /// <summary>
    /// Provides static methods for converting primitive types to and from little-endian byte representation.
    /// </summary>
    public static class LittleEndianBitConverter
    {
        /// <summary>
        /// Copies the bytes of an integer into the buffer, in little-endian order.
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <param name="count">The number of bytes the integer consists of.</param>
        /// <param name="buffer">The buffer to copy into.</param>
        private static void CopyBytes(long value, int count, byte[] buffer)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[i] = unchecked((byte) value);
                value >>= 8;
            }
        }

        private static long FromBytes(byte[] buffer, int startIndex, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (startIndex < 0 || startIndex > buffer.Length - count)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            long result = 0;
            int end = startIndex + count;
            for (int position = end - 1; position >= startIndex; position--)
            {
                result = unchecked((result << 8) | buffer[position]);
            }
            return result;
        }

        #region Conversion to primitive types

        /// <summary>
        /// Constructs a <see cref="System.UInt64"/> from bytes at a given offset in a byte array.
        /// </summary>
        /// <param name="array">The array with the bytes to use.</param>
        /// <param name="startIndex">The offset at which the number starts.</param>
        /// <returns>The constructed number.</returns>
        public static ulong ToUInt64(byte[] array, int startIndex)
        {
            return unchecked((ulong) FromBytes(array, startIndex, 8));
        }

        /// <summary>
        /// Constructs a <see cref="System.Int64"/> from bytes at a given offset in a byte array.
        /// </summary>
        /// <param name="array">The array with the bytes to use.</param>
        /// <param name="startIndex">The offset at which the number starts.</param>
        /// <returns>The constructed number.</returns>
        public static long ToInt64(byte[] array, int startIndex)
        {
            return FromBytes(array, startIndex, 8);
        }

        /// <summary>
        /// Constructs a <see cref="System.Double"/> from bytes at a given offset in a byte array.
        /// </summary>
        /// <param name="array">The array with the bytes to use.</param>
        /// <param name="startIndex">The offset at which the number starts.</param>
        /// <returns>The constructed number.</returns>
        public static double ToDouble(byte[] array, int startIndex)
        {
            return BitConverter.Int64BitsToDouble(FromBytes(array, startIndex, 8));
        }

        /// <summary>
        /// Constructs a <see cref="System.UInt32"/> from bytes at a given offset in a byte array.
        /// </summary>
        /// <param name="array">The array with the bytes to use.</param>
        /// <param name="startIndex">The offset at which the number starts.</param>
        /// <returns>The constructed number.</returns>
        public static uint ToUInt32(byte[] array, int startIndex)
        {
            return unchecked((uint) FromBytes(array, startIndex, 4));
        }

        /// <summary>
        /// Constructs a <see cref="System.Int32"/> from bytes at a given offset in a byte array.
        /// </summary>
        /// <param name="array">The array with the bytes to use.</param>
        /// <param name="startIndex">The offset at which the number starts.</param>
        /// <returns>The constructed number.</returns>
        public static int ToInt32(byte[] array, int startIndex)
        {
            return unchecked((int) FromBytes(array, startIndex, 4));
        }

        /// <summary>
        /// Constructs a <see cref="System.UInt16"/> from bytes at a given offset in a byte array.
        /// </summary>
        /// <param name="array">The array with the bytes to use.</param>
        /// <param name="startIndex">The offset at which the number starts.</param>
        /// <returns>The constructed number.</returns>
        public static ushort ToUInt16(byte[] array, int startIndex)
        {
            return unchecked((ushort) FromBytes(array, startIndex, 2));
        }

        /// <summary>
        /// Constructs a <see cref="System.Int16"/> from bytes at a given offset in a byte array.
        /// </summary>
        /// <param name="array">The array with the bytes to use.</param>
        /// <param name="startIndex">The offset at which the number starts.</param>
        /// <returns>The constructed number.</returns>
        public static short ToInt16(byte[] array, int startIndex)
        {
            return unchecked((short) FromBytes(array, startIndex, 2));
        }

        /// <summary>
        /// Constructs a <see cref="System.Boolean"/> from a byte at a given offset in a byte array.
        /// </summary>
        /// <param name="array">The array with the byte to use.</param>
        /// <param name="startIndex">The offset of the byte.</param>
        /// <returns>The boolean.</returns>
        public static bool ToBoolean(byte[] array, int startIndex)
        {
            return BitConverter.ToBoolean(array, startIndex);
        }

        #endregion

        #region GetBytes overload~

        /// <summary>
        /// Gets the little-endian byte representation of a <see cref="System.UInt64"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] GetBytes(ulong value)
        {
            return GetBytes(unchecked((long) value), 8);
        }

        /// <summary>
        /// Gets the little-endian byte representation of a <see cref="System.UInt64"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] GetBytes(long value)
        {
            return GetBytes(value, 8);
        }

        /// <summary>
        /// Gets the little-endian byte representation of a <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] GetBytes(double value)
        {
            return GetBytes(BitConverter.DoubleToInt64Bits(value), 8);
        }

        /// <summary>
        /// Gets the little-endian byte representation of a <see cref="System.UInt32"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] GetBytes(uint value)
        {
            return GetBytes(value, 4);
        }

        /// <summary>
        /// Gets the little-endian byte representation of a <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] GetBytes(int value)
        {
            return GetBytes(value, 4);
        }

        /// <summary>
        /// Gets the little-endian byte representation of a <see cref="System.UInt16"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] GetBytes(ushort value)
        {
            return GetBytes(value, 2);
        }

        /// <summary>
        /// Gets the little-endian byte representation of a <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] GetBytes(short value)
        {
            return GetBytes(value, 2);
        }

        /// <summary>
        /// Gets the byte representation of a <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The byte representation.</returns>
        public static byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        private static byte[] GetBytes(long value, int byteCount)
        {
            var buffer = new byte[byteCount];
            CopyBytes(value, byteCount, buffer);
            return buffer;
        }

        #endregion
    }
}