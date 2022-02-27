using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Bespoke.Common.Net;

namespace Bespoke.Osc
{
    /// <summary>
    /// Represents a TCP/IP client-side connection.
    /// </summary>
    public class OscClient
    {
        /// <summary>
        /// Gets the IP address of the server-side of the connection.
        /// </summary>
        public IPAddress RemoteIPAddress { get; private set; }        

        /// <summary>
        /// Gets the port of the server-side of the connection.
        /// </summary>
        public int RemotePort { get; private set; }        

        /// <summary>
        /// Gets the underlying <see cref="TcpConnection"/>.
        /// </summary>
        public TcpConnection Connection { get; private set; }

		/// <summary>
		/// Gets ths connected status of the underlying Tcp socket.
		/// </summary>
		public bool IsConnected
		{
			get
			{
				return (Connection != null ? Connection.Client.Connected : false);
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="OscClient"/> class.
        /// </summary>
        public OscClient()
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="OscClient"/> class.
		/// </summary>
		/// <param name="connection">The <see cref="TcpConnection"/> object associated with this instance.</param>
		public OscClient(TcpConnection connection)
		{
			Connection = connection;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="OscClient"/> class.
        /// </summary>
        /// <param name="serverEndPoint">The server-side endpoint of the connection.</param>
        public OscClient(IPEndPoint serverEndPoint)
            : this(serverEndPoint.Address, serverEndPoint.Port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OscClient"/> class.
        /// </summary>
        /// <param name="serverIPAddress">The server-side IP address of the connection.</param>
        /// <param name="serverPort">The server-side port of the connection.</param>
        public OscClient(IPAddress serverIPAddress, int serverPort)
            : this()
        {
            RemoteIPAddress = serverIPAddress;
            RemotePort = serverPort;
        }

        /// <summary>
        /// Connect to the previously specified server-side endpoint.
        /// </summary>
        public void Connect()
        {
            Connect(RemoteIPAddress, RemotePort);
        }

        /// <summary>
        /// Connect to the previously specified server-side endpoint.
        /// </summary>
        /// <param name="serverEndPoint">The server-side endpoint to connect to.</param>
        public void Connect(IPEndPoint serverEndPoint)
        {
            Connect(serverEndPoint.Address, serverEndPoint.Port);
        }

        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="serverIPAddress">The server-side IP address to connect to.</param>
        /// <param name="serverPort">The server-side port to connect to.</param>
        public void Connect(IPAddress serverIPAddress, int serverPort)
        {
            RemoteIPAddress = serverIPAddress;
            RemotePort = serverPort;

			if (Connection == null)
			{
				TcpClient client = new TcpClient();
				client.Connect(RemoteIPAddress, RemotePort);
				Connection = new TcpConnection(client.Client, OscPacket.LittleEndianByteOrder);
			}
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public void Close()
        {
			if (Connection != null)
			{
				Connection.Dispose();
				Connection = null;
			}
        }

        /// <summary>
        /// Send an OscPacket over the connection.
        /// </summary>
        /// <param name="packet">The <see cref="OscPacket"/> to send.</param>
        public void Send(OscPacket packet)
        {
            byte[] packetData = packet.ToByteArray();
			Connection.Writer.Write(OscPacket.ValueToByteArray(packetData));
        }
    }
}
