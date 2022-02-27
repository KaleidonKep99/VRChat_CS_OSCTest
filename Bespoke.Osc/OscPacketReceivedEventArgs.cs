using System;
using Bespoke.Common;

namespace Bespoke.Osc
{
	/// <summary>
	/// Arguments for packet received events.
	/// </summary>
	public class OscPacketReceivedEventArgs : EventArgs
	{
		/// <summary>
        /// Gets the <see cref="OscPacket"/> received.
		/// </summary>
		public OscPacket Packet { get; private set; }

		/// <summary>
        /// Initializes a new instance of the <see cref="OscMessageReceivedEventArgs"/> class.
		/// </summary>
        /// <param name="packet">The <see cref="OscPacket"/> received.</param>
		public OscPacketReceivedEventArgs(OscPacket packet)
		{
			Assert.ParamIsNotNull(packet);

			Packet = packet;
		}
	}
}
