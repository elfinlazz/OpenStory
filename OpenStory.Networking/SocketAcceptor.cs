﻿using System;
using System.Net;
using System.Net.Sockets;

namespace OpenStory.Networking
{
    /// <summary>
    /// Represents a simple connection acceptor.
    /// </summary>
    public sealed class SocketAcceptor : IDisposable
    {
        /// <summary>
        /// The event raised when a new socket connection has been accepted.
        /// </summary>
        public event EventHandler<SocketEventArgs> SocketAccepted;

        /// <summary>
        /// The event raised when a socket error occurs.
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> SocketError;

        private readonly IPEndPoint localEndPoint;
        private SocketAsyncEventArgs socketArgs;
        private Socket acceptSocket;
        private bool isDisposed;

        /// <summary>
        /// The <see cref="IPAddress"/> to which this SocketAcceptor is bound.
        /// </summary>
        public IPAddress Address { get; private set; }

        /// <summary>
        /// The port to which this SocketAcceptor is bound.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Initializes a new instance of SocketAcceptor and binds it to the given port.
        /// </summary>
        /// <param name="address">The <see cref="IPAddress"/> to accept connections through.</param>
        /// <param name="port">The port to listen to for incoming connections.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="address"/> is <c>null</c>.
        /// </exception>
        public SocketAcceptor(IPAddress address, int port)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            this.isDisposed = false;

            this.Port = port;
            this.Address = address;

            this.localEndPoint = new IPEndPoint(this.Address, this.Port);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.DisposeSocketIfNotNull();
            this.isDisposed = true;
        }

        /// <summary>
        /// Starts the process of accepting connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="SocketAccepted"/> event has no subscribers.
        /// </exception>
        public void Start()
        {
            if (this.SocketAccepted == null)
            {
                throw new InvalidOperationException("'SocketAccepted' has no subscribers.");
            }

            this.acceptSocket = this.GetAcceptSocket();

            this.BeginAccept();
        }

        /// <summary>
        /// Halts the process of accepting connections.
        /// </summary>
        public void Stop()
        {
            this.DisposeSocketIfNotNull();
        }

        private Socket GetAcceptSocket()
        {
            this.DisposeSocketIfNotNull();

            this.socketArgs = new SocketAsyncEventArgs();
            this.socketArgs.Completed += (sender, eventArgs) => this.EndAcceptAsynchronous(eventArgs);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(this.localEndPoint);
            socket.Listen(100);

            return socket;
        }

        private void BeginAccept()
        {
            this.socketArgs.AcceptSocket = null;

            try
            {
                while (!this.acceptSocket.AcceptAsync(this.socketArgs))
                {
                    if (!this.EndAcceptSynchronous(this.socketArgs)) break;
                }
            }
            catch (ObjectDisposedException)
            {
                this.acceptSocket.Close();
            }
        }

        private bool EndAcceptSynchronous(SocketAsyncEventArgs eventArgs)
        {
            if (eventArgs.SocketError != System.Net.Sockets.SocketError.Success)
            {
                this.HandleError(eventArgs.SocketError);
                return false;
            }

            Socket clientSocket = eventArgs.AcceptSocket;
            var socketEventArgs = new SocketEventArgs(clientSocket);
            this.SocketAccepted(this, socketEventArgs);
            return true;
        }

        private void EndAcceptAsynchronous(SocketAsyncEventArgs eventArgs)
        {
            if (this.EndAcceptSynchronous(eventArgs))
            {
                this.BeginAccept();
            }
        }

        private void HandleError(SocketError error)
        {
            if (this.SocketError != null)
            {
                this.SocketError(this, new SocketErrorEventArgs(error));
            }

            // OperationAborted comes up when we closed the socket manually.
            if (error != System.Net.Sockets.SocketError.OperationAborted)
            {
                this.Stop();
            }
        }

        private void DisposeSocketIfNotNull()
        {
            if (this.acceptSocket != null)
            {
                this.acceptSocket.Dispose();
            }

            if (this.socketArgs != null)
            {
                this.socketArgs.Dispose();
            }
        }
    }
}