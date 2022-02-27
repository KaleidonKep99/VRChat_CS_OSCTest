using System;

namespace Bespoke.Common.Net
{
    /// <summary>
    /// Data for Tcp connection-related events.
    /// </summary>
    public class TcpConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the associated connection.
        /// </summary>
        public TcpConnection Connection { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The associated connection.</param>
        public TcpConnectionEventArgs(TcpConnection connection)
        {
            Connection = connection;
        }
    }
}