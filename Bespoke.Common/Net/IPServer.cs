using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Bespoke.Common.Net
{
    /// <summary>
    /// Helper methods for IP servers.
    /// </summary>
    public abstract class IPServer
    {
        /// <summary>
        /// Get the local IP addresses bound to this computer.
        /// </summary>
        /// <returns>The list of IP addresses bound to this computer.</returns>
        /// <exception cref="Exception">Thrown if no local IP addresses are found.</exception>
        public static IPAddress[] GetLocalIPAddresses()
        {
            IPAddress[] localAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            if (localAddresses.Length == 0)
            {
                throw new Exception("No local IP Address address found.");
            }

            return localAddresses;
        }

        /// <summary>
        /// Determine if the specified Udp end point is available.
        /// </summary>
        /// <param name="ipAddress">The IP address to check.</param>
        /// <param name="port">The port to check.</param>
        /// <returns>true if the specified end point is available; otherwise, false.</returns>
        public static bool IsUdpEndPointAvailable(IPAddress ipAddress, int port)
        {
            return IsUdpEndPointAvailable(new IPEndPoint(ipAddress, port));
        }

        /// <summary>
        /// Determine if the specified Udp end point is available.
        /// </summary>
        /// <param name="ipEndPoint">The IP end point to check.</param>
        /// <returns>true if the specified end point is available; otherwise, false.</returns>
        public static bool IsUdpEndPointAvailable(IPEndPoint ipEndPoint)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] activeUdpListeners = ipGlobalProperties.GetActiveUdpListeners();

            return activeUdpListeners.Any(activeUdpListener => (activeUdpListener.Address == ipEndPoint.Address) && (activeUdpListener.Port == ipEndPoint.Port));
        }

        /// <summary>
        /// Determine if the specified Tcp end point is available.
        /// </summary>
        /// <param name="ipAddress">The IP address to check.</param>
        /// <param name="port">The port to check.</param>
        /// <returns>true if the specified end point is available; otherwise, false.</returns>
        public static bool IsTcpEndPointAvailable(IPAddress ipAddress, int port)
        {
            return IsTcpEndPointAvailable(new IPEndPoint(ipAddress, port));
        }

        /// <summary>
        /// Determine if the specified Tcp end point is available.
        /// </summary>
        /// <param name="ipEndPoint">The IP end point to check.</param>
        /// <returns>true if the specified end point is available; otherwise, false.</returns>
        public static bool IsTcpEndPointAvailable(IPEndPoint ipEndPoint)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] activeTcpListeners = ipGlobalProperties.GetActiveTcpListeners();

            return activeTcpListeners.Any(activeUdpListener => (activeUdpListener.Address == ipEndPoint.Address) && (activeUdpListener.Port == ipEndPoint.Port));
        }
    }
}
