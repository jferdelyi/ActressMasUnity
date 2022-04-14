using System;

namespace ActressMasWrapper {

	/// <summary>
	/// Some tools
	/// </summary>
	public class Tools {

		/// <summary>
		/// Static random
		/// </summary>
		public static Random Rand = new();

		/// <summary>
		/// Number of created agents
		/// </summary>
		protected static long msTotalCount = 0;

		/// <summary>
		/// Create agent name
		/// </summary>
		/// <returns>New name</returns>
		public static string CreateName() {
			return "Agent[" + ++msTotalCount + "]";
		}
	}
}
