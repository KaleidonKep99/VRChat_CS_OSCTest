using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Bespoke.Common.Net
{
    /// <summary>
    /// A multi-threaded Tcp server.
    /// </summary>
    /// <remarks>Data can be received automatically, by the connections established by the <see cref="TcpServer"/> 
    /// by setting <see cref="ReceiveDataInline"/> to true during instantiation (the default behavior). This establishes
    /// asynchronous reception of Tcp messages framed using a 4-byte integer containing the length of the message. The
    /// <see cref="DataReceived"/> event is raised as complete messages are received. Users can perform their own
    /// message handling by setting <see cref="ReceiveDataInline"/> to false and listening for <see cref="Connected" /> events.</remarks>
    public class TcpServer : IPServer, IDisposable
    {
        /// <summary>
        /// The maximum number of connections that can be pending.
        /// </summary>
        public static readonly int MaxPendingConnections = 3;

        #region Events

        /// <summary>
        /// Raised when a connection is established.
        /// </summary>
        public event EventHandler<TcpConnectionEventArgs> Connected;

        /// <summary>
        /// Raised when a connection is disconnected.
        /// </summary>
        public event EventHandler<TcpConnectionEventArgs> Disconnected;

        /// <summary>
        /// Raised when data is received.
        /// </summary>
        public event EventHandler<TcpDataReceivedEventArgs> DataReceived;

        #endregion

        /// <summary>
        /// Gets the IP address the Tcp server is bound to.
        /// </summary>
        public IPAddress IPAddress { get; private set; }

        /// <summary>
        /// Gets the port the Tcp server is bound to.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets the state of the server.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return mAcceptingConnections;
            }
        }

        /// <summary>
        /// Gets the number of active connections.
        /// </summary>
        public int ActiveConnectionCount
        {
            get
            {
				lock (mClientConnections)
				{
					return mClientConnections.Count; 
				}
            }
        }

        /// <summary>
        /// Gets the list of active connections.
        /// </summary>
        public ReadOnlyCollection<TcpConnection> ActiveConnections
        {
            get
            {
                lock (mClientConnections)
                {
                    return mClientConnections.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Gets the data reception mode applied to the Tcp server.
        /// </summary>
        public bool ReceiveDataInline { get; private set; }

        /// <summary>
        /// Gets or sets the expected endianness of integral value types.
        /// </summary>
        public bool LittleEndianByteOrder { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <param name="port">The port to bind to.</param>
        /// <remarks>Uses the loopback address, inline data reception and little endian byte order.</remarks>
        public TcpServer(int port)
            : this(IPAddress.Loopback, port, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <param name="ipAddress">The IP address to bind to.</param>
        /// <param name="port">The port to bind to.</param>
        /// <param name="receiveDataInline">The mode of automatic data reception.</param>
        /// <param name="littleEndianByteOrder">The expected endianness of integral value types.</param>
        public TcpServer(IPAddress ipAddress, int port, bool receiveDataInline = true, bool littleEndianByteOrder = true)
        {
            Port = port;
            IPAddress = ipAddress;
            ReceiveDataInline = receiveDataInline;
            mClientConnections = new List<TcpConnection>();
            mConnectionsToClose = new List<TcpConnection>();
            mIsShuttingDown = false;
            LittleEndianByteOrder = littleEndianByteOrder;
        }

        /// <summary>
        /// Release the resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            if (mIsShuttingDown == false)
            {
                Stop();
            }

            lock (mClientConnections)
            {
                foreach (TcpConnection connection in mClientConnections)
                {
                    connection.Dispose();
                }

                mClientConnections.Clear();
                mClientConnections = null;
            }

            lock (mConnectionsToClose)
            {
                mConnectionsToClose.Clear();
                mConnectionsToClose = null;
            }
        }

        /// <summary>
        /// Start the Tcp server.
        /// </summary>
        /// <remarks>This is a non-blocking (asynchronous) call.</remarks>
        /// <returns>A <seealso cref="Task"/>Task associated with the method.</returns>
        public async Task Start()
        {
            mTcpListener = null;

            try
            {
                mIsShuttingDown = false;
                mAcceptingConnections = true;

                mTcpListener = new TcpListener(IPAddress, Port);
                mTcpListener.Start(MaxPendingConnections);

                while (true)
                {
                    Socket socket = await mTcpListener.AcceptSocketAsync();
                    if (socket == null)
                    {
                        break;
                    }

                    await Task.Run(() =>
                    {
                        TcpConnection connection = new TcpConnection(socket, LittleEndianByteOrder);
                        connection.Disconnected += new EventHandler<TcpConnectionEventArgs>(OnDisconnected);
                        connection.DataReceived += new EventHandler<TcpDataReceivedEventArgs>(OnDataReceived);

                        lock (mClientConnections)
                        {
                            mClientConnections.Add(connection);
                        }

                        if (ReceiveDataInline)
                        {
                            connection.ReceiveDataAsync();
                        }

                        OnConnected(new TcpConnectionEventArgs(connection));
                    });
                }
            }
            catch (ObjectDisposedException)
            {
                // Supress exception
            }
            finally
            {
                lock (mClientConnections)
                {
                    lock (mConnectionsToClose)
                    {
                        foreach (TcpConnection connection in mClientConnections)
                        {
                            mConnectionsToClose.Add(connection);
                        }
                    }

                    CloseMarkedConnections();
                }

                mIsShuttingDown = true;
            }
        }

        /// <summary>
        /// Stop the Tcp server.
        /// </summary>
        public void Stop()
        {
			if (mTcpListener != null)
			{
				mTcpListener.Stop();
			}
        }

        /// <summary>
        /// Close an active connection.
        /// </summary>
        /// <param name="connection">The connection to close.</param>
        public void CloseConnection(TcpConnection connection)
        {
            try
            {
                connection.Dispose();
            }
            catch
            {
                // Igore any exceptions
            }
            finally
            {
                lock (mClientConnections)
                {
                    mClientConnections.Remove(connection);
                }
            }
        }

        #region Private Methods

        /// <summary>
        /// Raise the Connected event.
        /// </summary>
        /// <param name="e">An <see cref="TcpConnectionEventArgs"/> object that contains the event data.</param>
        private void OnConnected(TcpConnectionEventArgs e)
        {
            if (Connected != null)
            {
                Connected(this, e);
            }
        }

        /// <summary>
        /// Raise the Disconnected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">An <see cref="TcpConnectionEventArgs"/> object that contains the event data.</param>
        private void OnDisconnected(object sender, TcpConnectionEventArgs e)
        {
            if (Disconnected != null)
            {
                CloseConnection(e.Connection);
                Disconnected(this, e);
            }
        }

        /// <summary>
        /// Raise the DataReceived event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">An <see cref="TcpDataReceivedEventArgs"/> object that contains the event data.</param>
        private void OnDataReceived(object sender, TcpDataReceivedEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        /// <summary>
        /// Close marked connections.
        /// </summary>
        private void CloseMarkedConnections()
        {
            lock (mConnectionsToClose)
            {
                foreach (TcpConnection connection in mConnectionsToClose)
                {
                    CloseConnection(connection);
                }

                mConnectionsToClose.Clear();
            }
        }

        #endregion

        private TcpListener mTcpListener;
        private List<TcpConnection> mClientConnections;
        private List<TcpConnection> mConnectionsToClose;
        private volatile bool mIsShuttingDown;
        private volatile bool mAcceptingConnections;
    }
}
