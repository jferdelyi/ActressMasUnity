using ActressMas;

namespace ActressMasWrapper {

	/// <summary>
	/// The ActressMas environment Unity wrapper
	/// </summary>
	public abstract class EnvironmentWrapper : EnvironmentMas {

		/// <summary>
		/// The GameObject behaviour
		/// </summary>
		public GraphicalEnvironment Graphical { get; protected set; }

		/// <summary>
		/// Number of turns
		/// </summary>
		public int Turn { get; protected set; }

		/// <summary>
		/// Sum time
		/// </summary>
		public double TotalTime { get; protected set; }

		/// <summary>
		/// Simulation per seconds
		/// </summary>
		public int SimulationPerSeconds { get; protected set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pSimulationPerSeconds">Number of turn per seconds</param>
		/// <param name="pGraphical">The environment behaviour</param>
		/// <param name="pDelayAfterTurn">A delay (in miliseconds) after each turn</param>
		/// <param name="pRandomOrder">Whether the agents should be run in a random order (different each turn) or sequentially. If the execution is parallel, agents are always run in random order</param>
		/// <param name="pRand">A random number generator for non-deterministic but repeatable experiments. It should instantiated using a seed. If it is null, a new Random object is created and used</param>
		/// <param name="pParallel">Whether agent behaviors are executed in parallel or sequentially. The code of a single agent in a turn is always executed sequentially</param>
		public EnvironmentWrapper(int pSimulationPerSeconds, GraphicalEnvironment pGraphical, int pDelayAfterTurn = 0, bool pRandomOrder = true, System.Random pRand = null, bool pParallel = true)
			: base(pDelayAfterTurn, pRandomOrder, pRand, pParallel) {
			SimulationPerSeconds = pSimulationPerSeconds;
			Graphical = pGraphical;
			Turn = 0;
		}

		/// <summary>
		/// Update simulation
		/// </summary>
		/// <param name="pDeltaTime">The delta time</param>
		public void RunTurn(double pDeltaTime) {
			TotalTime += pDeltaTime;

			if (TotalTime >= (1 / (double)SimulationPerSeconds)) {
				TotalTime %= (1 / (double)SimulationPerSeconds);
				base.RunTurn(Turn);
			}

			Turn++;
		}

		/// <summary>
		/// Add data (graphical info)
		/// </summary>
		/// <param name="pName">The name of the data to add</param>
		/// <param name="pDataToWrap">The data to add</param>
		/// <returns>True if added</returns>
		public bool AddGraphicalData(string pName, dynamic pDataToWrap) {
			return Graphical.AddGraphicalData(pName, pDataToWrap);
		}

		/// <summary>
		/// Get data (graphical info)
		/// </summary>
		/// <param name="pName">The name of the data</param>
		/// <returns>Data</returns>
		public dynamic GetGraphicalData(string pName) {
			return Graphical.GetGraphicalData(pName);
		}

		/// <summary>
		/// Erase data (graphical info)
		/// </summary>
		/// <param name="pName">The name of the data</param>
		public void EraseData(string pName) {
			Graphical.EraseData(pName);
		}
	}
}
