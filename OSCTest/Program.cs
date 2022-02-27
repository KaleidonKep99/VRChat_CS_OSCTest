using System.Net;
using System.Net.Sockets;
using Bespoke.Osc;

namespace Test01
{
    internal class Program
    {
        static readonly IPAddress IP = IPAddress.Loopback;
        static readonly int Port = 9000;
        static readonly IPEndPoint VRChat = new IPEndPoint(IP, Port);

        static void Main(string[] args)
        {
            // Create a bogus port for the client
            OscPacket.UdpClient = new UdpClient(10025);

            int A = 0;

            while (true)
            {
                try
                {
                    if (A > 7) A = 0;

                    // Send the packet with the following values
                    SendPacket("GestureLeft", A);

                    Console.Clear();
                    Console.WriteLine(String.Format("Sent {0} to GestureLeft!", A));

                    Thread.Sleep(600);

                    A++;
                }
                catch { 
                    Console.Clear();
                    Console.WriteLine("Error!"); 
                }
            }
        }

        static void SendPacket(string Target, object Param)
        {
            try
            {
                // Check if Param is of one of the following supported types
                if (Param.GetType() != typeof(int) &&
                    Param.GetType() != typeof(float) &&
                    Param.GetType() != typeof(bool))
                    throw new Exception(String.Format("Param of type {0} is not supported by VRChat!", Param.GetType().ToString()));

                // Create a bundle that contains the target address and port (VRChat works on localhost:9000)
                OscBundle VRBundle = new OscBundle(VRChat);

                // Create the message, and append the parameter to it
                OscMessage Message = new OscMessage(VRChat, String.Format("/avatar/parameters/{0}", Target));
                Message.Append(Param);

                // Append the message to the bundle
                VRBundle.Append(Message);

                // Send the bundle to the target address and port
                VRBundle.Send(VRChat);
            }
            catch
            {
                Console.WriteLine("Something went wrong! Maybe the Param was of an unsupported type?");
            }
        }
    }
}
