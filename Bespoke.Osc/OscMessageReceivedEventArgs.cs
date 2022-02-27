using System;
using Bespoke.Common;

namespace Bespoke.Osc
{
	/// <summary>
	/// Arguments for message received events.
	/// </summary>
	public class OscMessageReceivedEventArgs : EventArgs
	{
		/// <summary>
        /// Gets the <see cref="OscMessage"/> received.
		/// </summary>
		public OscMessage Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OscMessageReceivedEventArgs"/> class.
		/// </summary>
        /// <param name="message">The <see cref="OscMessage"/> received.</param>
		public OscMessageReceivedEventArgs(OscMessage message)
		{
			Assert.ParamIsNotNull(message);

			Message = message;
		}
	}
}
