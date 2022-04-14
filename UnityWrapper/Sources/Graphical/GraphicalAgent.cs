using UnityEngine;

namespace ActressMasWrapper {

	/// <summary>
	/// The graphic agent behaviour<
	/// /summary>
	public abstract class GraphicalAgent : MonoBehaviour {

		/// <summary>
		/// ActressMas agent
		/// </summary>
		public AgentWrapper Agent { get; set; }

		/// <summary>
		/// Kill the agent
		/// </summary>
		public void Kill() {
			Destroy(gameObject);
			Agent = null;
		}

		/// <summary>
		/// On destroy
		/// </summary>
		void OnDestroy() {
			if (Agent != null) {
				Agent.Stop();
			}
		}
	}
}
