using System;
using Bespoke.Common;

namespace Bespoke.Osc
{
	/// <summary>
	/// Arguments for bundle received events.
	/// </summary>
	public class OscBundleReceivedEventArgs : EventArgs
	{
		/// <summary>
        /// Gets the <see cref="OscBundle"/> received.
		/// </summary>
        public OscBundle Bundle { get; private set; }

		/// <summary>
        /// Initializes a new instance of the <see cref="OscBundleReceivedEventArgs"/> class.
		/// </summary>
		/// <param name="bundle">The <see cref="OscBundle"/> received.</param>
		public OscBundleReceivedEventArgs(OscBundle bundle)
		{
			Assert.ParamIsNotNull(bundle);

			Bundle = bundle;
		}
	}
}
