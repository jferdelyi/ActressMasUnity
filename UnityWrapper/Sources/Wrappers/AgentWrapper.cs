using ActressMas;
using UnityEngine;

namespace ActressMasWrapper {

	/// <summary>
	/// The ActressMas agent Unity wrapper
	/// </summary>
	public abstract class AgentWrapper : Agent {

		/// <summary>
		/// If true the agent will be deleted
		/// </summary>
		public bool IsOnDestroy { get; private set; } = false;

		/// <summary>
		/// The GameObject behaviour
		/// </summary>
		public GraphicalAgent Graphical { get; private set; }

		/// <summary>
		/// Transform wrapper
		/// </summary>
		public Transform Transform { get { return Graphical.transform; } }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pGraphical">The agent behaviour</param>
		public AgentWrapper(GraphicalAgent pGraphical = null)
			: base() {
			Graphical = pGraphical;
		}

		/// <summary>
		/// Kill the agent
		/// </summary>
		public void Kill() {
			IsOnDestroy = true;
			if (Graphical != null) {
				Graphical.Agent = null;
			}
			Stop();
		}

		/// <summary>
		/// Teardown the agent
		/// </summary>
		public abstract void Teardown();
	}
}
