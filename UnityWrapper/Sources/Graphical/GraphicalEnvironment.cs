using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace ActressMasWrapper {

	/// <summary>
	/// The graphic environment behaviour
	/// </summary>
	public abstract class GraphicalEnvironment : MonoBehaviour {

		/// <summary>
		/// ActressMas environment
		/// </summary>
		public EnvironmentWrapper Environment { get; protected set; }

		/// <summary>
		/// Agent data in order to instanciated new graphical data
		/// </summary>
		protected ConcurrentDictionary<string, dynamic> mAgentDataWrapper = new();

		/// <summary>
		/// Called once per frame
		/// </summary>
		public void Update() {
			Environment.RunTurn(Time.deltaTime);
			CheckNewData();
		}

		/// <summary>
		/// Perform additional processing after the simulation has finished
		/// </summary>
		public void OnDestroy() {
			Environment.SimulationFinished();
		}

		/// <summary>
		/// Add data (graphical info)
		/// </summary>
		/// <param name="pName">The name of the data to add</param>
		/// <param name="mDataToWrap">The data to add</param>
		/// <returns>True if added</returns>
		public bool AddGraphicalData(string pName, dynamic mDataToWrap) {
			try {
				mAgentDataWrapper.TryAdd(pName, mDataToWrap);
				return true;
			} catch (ArgumentException) {
				return false;
			}
		}

		/// <summary>
		/// Get data (graphical info)
		/// </summary>
		/// <param name="pName">The name of the data</param>
		/// <returns>Data</returns>
		public dynamic GetGraphicalData(string pName) {
			mAgentDataWrapper.TryGetValue(pName, out dynamic lReturn);
			return lReturn;
		}

		/// <summary>
		/// Erase data (graphical info)
		/// </summary>
		/// <param name="pName">The name of the data</param>
		public void EraseData(string pName) {
			mAgentDataWrapper.TryRemove(pName, out _);
		}

		/// <summary>
		/// In order to check if there is new agent to instanciated
		/// </summary>
		protected abstract void CheckNewData();
	}
}
