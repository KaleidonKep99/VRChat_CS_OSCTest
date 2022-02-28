using System.Net;
using System.Net.Sockets;
using System.Text;
using Bespoke.Osc;

namespace Test01
{
    internal class Program
    {
        static readonly IPAddress IP = IPAddress.Loopback;
        static readonly int Port = 9000;
        static readonly IPEndPoint VRChat = new IPEndPoint(IP, Port);
        static Random Random = new Random();

        static void Main(string[] args)
        {
            // Create a bogus port for the client
            OscPacket.UdpClient = new UdpClient(10025);

            while (true)
            {
                try
                {
                    // Build the message(s) by providing your parameter target(s) and its (or their) value(s)
                    VRChatMessage Msg1 = new VRChatMessage("GestureLeftWeight", (float)Random.NextDouble());
                    VRChatMessage Msg2 = new VRChatMessage("GestureRightWeight", (float)Random.NextDouble());

                    // Send the packet(s) using the SendPacket function
                    SendPacket(Msg1, Msg2);

                    LogToConsole("Sent to GestureLeftWeight and GestureRightWeight!", Msg1, Msg2);

                    Thread.Sleep(600);
                }
                catch {
                    LogToConsole("Error!"); 
                }
            }
        }

        static void SendPacket(params VRChatMessage[] Params)
        {
            foreach (var Param in Params)
            {
                try
                {
                    // Check if there's a valid target
                    if (Param.Parameter == null || string.IsNullOrEmpty(Param.Parameter))
                        throw new Exception("Parameter target not set!");

                    // Check if Parameter is not null and of one of the following supported types
                    if (Param.Data != null &&
                        Param.Data.GetType() != typeof(int) &&
                        Param.Data.GetType() != typeof(float) &&
                        Param.Data.GetType() != typeof(bool))
                        throw new Exception(String.Format("Param of type {0} is not supported by VRChat!", Param.Data.GetType()));

                    // Create a bundle that contains the target address and port (VRChat works on localhost:9000)
                    OscBundle VRBundle = new OscBundle(VRChat);

                    // Create the message, and append the parameter to it
                    OscMessage Message = new OscMessage(VRChat, String.Format("/avatar/parameters/{0}", Param.Parameter));
                    Message.Append(Param.Data);

                    // Append the message to the bundle
                    VRBundle.Append(Message);

                    // Send the bundle to the target address and port
                    VRBundle.Send(VRChat);

                }
                catch (Exception ex)
                {
                    LogToConsole(ex.ToString());
                }
            }
        }

        static void LogToConsole(string Message, params VRChatMessage[] Parameters)
        {
            StringBuilder MessageBuilder = new StringBuilder();

            MessageBuilder.Append(String.Format("{0} - {1}", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), Message));

            if (Parameters.Length > 0)
            {
                MessageBuilder.Append(" (");

                var LastParam = Parameters[Parameters.Length - 1];
                foreach (var Parameter in Parameters)
                {
                    MessageBuilder.Append(String.Format("{0} of type {1}", Parameter.Data, Parameter.Data.GetType()));

                    if (Parameter != LastParam)
                        MessageBuilder.Append(", ");
                }

                MessageBuilder.Append(")");
            }

            Console.WriteLine(MessageBuilder.ToString());
        }
    }

    public class VRChatMessage
    {
        // The target of the data
        public string? Parameter { get; }

        // The data itself
        public object? Data { get; }

        public VRChatMessage(string A, object B)
        {
            Parameter = A;
            Data = B;
        }
    }
}
