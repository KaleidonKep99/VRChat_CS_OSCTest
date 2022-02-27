using System;

namespace Bespoke.Common.Net
{
    /// <summary>
    /// Data for Tcp data received events.
    /// </summary>
    public class TcpDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the associated connection.
        /// </summary>
        public TcpConnection Connection { get; private set; }

        /// <summary>
        /// Gets the associated data.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpDataReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The associated connection.</param>
        /// <param name="data">The associated data.</param>
        public TcpDataReceivedEventArgs(TcpConnection connection, byte[] data)
        {
            Connection = connection;
            Data = data;
        }
    }
}
