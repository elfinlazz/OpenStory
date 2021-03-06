using System;
using System.Collections.Concurrent;
using OpenStory.Common;
using OpenStory.Common.IO;
using OpenStory.Cryptography;
using OpenStory.Framework.Contracts;
using OpenStory.Networking;

namespace OpenStory.Server.Processing
{
    /// <summary>
    /// Represents an encrypted network session.
    /// </summary>
    internal sealed class ServerSession : EncryptedNetworkSession, IServerSession
    {
        private static readonly AtomicInteger RollingNetworkSessionId = new AtomicInteger(0);

        #region Fields and properties

        /// <inheritdoc />
        public event EventHandler ReadyForPush;

        /// <inheritdoc />
        public event EventHandler<PacketProcessingEventArgs> PacketProcessing;

        private readonly ConcurrentQueue<byte[]> packets;
        private readonly AtomicBoolean isPushing;
        private readonly IPacketCodeTable packetCodeTable;

        /// <inheritdoc />
        /// <remarks>
        /// This session identifier and the account session identifier are different things.
        /// </remarks>
        public int NetworkSessionId { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSession"/> class.
        /// </summary>
        public ServerSession(IPacketCodeTable packetCodeTable)
        {
            this.NetworkSessionId = RollingNetworkSessionId.Increment();

            this.isPushing = false;
            this.packets = new ConcurrentQueue<byte[]>();

            this.packetCodeTable = packetCodeTable;

            this.PacketReceived += this.HandlePacketReceived;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="endpointCrypto"/> or <paramref name="handshakeInfo"/> are <see langword="null"/>.
        /// </exception>
        public void Start(EndpointCrypto endpointCrypto, HandshakeInfo handshakeInfo)
        {
            Guard.NotNull(() => endpointCrypto, endpointCrypto);
            Guard.NotNull(() => handshakeInfo, handshakeInfo);

            this.ThrowIfNoPacketReceivedSubscriber();

            this.Crypto = endpointCrypto;

            byte[] handshake = ConstructHandshakePacket(handshakeInfo);
            this.Session.Start();
            this.Session.Write(handshake);
        }

        #region Outgoing logic

        private static byte[] ConstructHandshakePacket(HandshakeInfo handshakeInfo)
        {
            using (var builder = new PacketBuilder())
            {
                builder.WriteInt16(handshakeInfo.Header);
                builder.WriteInt16(handshakeInfo.Version);
                builder.WriteLengthString(handshakeInfo.Subversion);
                builder.WriteBytes(handshakeInfo.ClientIv);
                builder.WriteBytes(handshakeInfo.ServerIv);

                // Locale ID (used for localizations and test servers)
                builder.WriteByte(handshakeInfo.LocaleId);

                return builder.ToByteArray();
            }
        }

        #endregion

        #region Pushing events

        /// <summary>
        /// Starts the asynchronous packet sending process.
        /// </summary>
        public void Push()
        {
            if (this.PacketProcessing == null)
            {
                throw new InvalidOperationException(ServerStrings.PacketProcessingEventHasNoSubscriber);
            }

            // CompareExchange returns the original value, hence:
            // => true means we were already pushing, don't start a second one.
            // => false means we were not pushing and we just toggled it, so we should start now.
            if (!this.isPushing.CompareExchange(comparand: false, newValue: true))
            {
                this.StartPushing();
            }
        }

        private void StartPushing()
        {
            byte[] packet;
            bool hasPacket = this.packets.TryDequeue(out packet);

            // This can only be false when we are called by the asynchronous continuation,
            // which doesn't have a good way of communicating its results to us,
            // so it can't dequeue the next packet and pass it. So we do it here instead <3
            if (hasPacket)
            {
                // TryPushAsync will return false if we completed synchronously
                // and we would therefore like to try pushing another packet right away.
                while (!this.TryPushAsync(packet))
                {
                    // TryContinuePushSynchronous will return false if there are no more packets to push.
                    if (!this.TryContinuePushSynchronous(out packet))
                    {
                        break;
                    }
                }
            }
            else
            {
                this.isPushing.Exchange(newValue: false);
            }
        }

        /// <summary>
        /// Raises the <see cref="PacketProcessing"/> event for the specified packet data, if possible asynchronously.
        /// </summary>
        /// <param name="packet">The packet data to include for the raised event.</param>
        /// <returns>
        /// <see langword="false"/> if the data was handled synchronously and another call can be made; if there was a fatal error or the asynchronous call was successfully made, <see langword="false"/>.</returns>
        private bool TryPushAsync(byte[] packet)
        {
            var reader = new PacketReader(packet);

            ushort packetCode;
            if (!reader.TryReadUInt16(out packetCode))
            {
                this.Close();

                // Bad packet. We kill the session and stop pushing.
                return true;
            }

            string label;
            if (!this.packetCodeTable.TryGetIncomingLabel(packetCode, out label))
            {
                // Bad server. Take the blame and try the next one! :<
                return false;
            }

            // Invoke event asynchronously.
            var args = new PacketProcessingEventArgs(label, reader);
            var handler = this.PacketProcessing;
            AsyncCallback callback = this.ContinuePushAsynchronous;
            var asyncResult = handler.BeginInvoke(this, args, callback, null);

            // If we completed synchronously, we'll take another one right away.
            // Otherwise, leave it to the asynchronous continuation.
            return asyncResult.CompletedSynchronously;
        }

        /// <summary>
        /// Attempts to retrieve another packet for processing.
        /// </summary>
        /// <remarks>
        /// If there were no packets to process, <paramref name="packet"/> will be <see langword="null"/> and <see cref="isPushing"/> will be set to false.
        /// </remarks>
        /// <param name="packet">A variable to hold the retrieved packet data.</param>
        /// <returns>
        /// <see langword="true"/> if there is more data to process; otherwise, <see langword="false"/>.
        /// </returns>
        private bool TryContinuePushSynchronous(out byte[] packet)
        {
            bool hasNext = this.packets.TryDequeue(out packet);
            if (!hasNext)
            {
                this.isPushing.Exchange(newValue: false);
            }

            return hasNext;
        }

        /// <summary>
        /// Continues packet processing after a successful asynchronous push is complete.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> object for the asynchronous push.</param>
        private void ContinuePushAsynchronous(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                this.StartPushing();
            }
        }

        #endregion

        #region Packet arrival handling

        private void HandlePacketReceived(object sender, PacketReceivedEventArgs e)
        {
            var bytes = e.Reader.ReadFully();

            this.packets.Enqueue(bytes);

            if (!this.isPushing.Value)
            {
                this.OnReadyForPush();
            }
        }

        private void OnReadyForPush()
        {
            var handler = this.ReadyForPush;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
