/**************************************************************************
 *                                                                        *
 *  Description: ActressMas multi-agent framework                         *
 *  Website:     https://github.com/florinleon/ActressMas                 *
 *  Copyright:   (c) 2018-2021, Florin Leon                               *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ActressMas {

	/// <summary>
	/// An abstract base class for the multiagent environment, where all the agents are executed.
	/// </summary>
	public abstract class EnvironmentMas {

		/// <summary>
		/// The general random
		/// </summary>
		protected Random mRand;

		/// <summary>
		/// The concurence locker
		/// </summary>
		private readonly object mLocker = new object();

		/// <summary>
		/// A delay (in miliseconds) after each turn
		/// </summary>
		private int mDelayAfterTurn;

		/// <summary>
		/// Whether agent behaviors are executed in parallel or sequentially. The code of a single agent in a turn is always executed sequentially
		/// </summary>
		private bool mParallel;

		/// <summary>
		/// Whether the agents should be run in a random order (different each turn) or sequentially. If the execution is parallel, agents are always run in random order
		/// </summary>
		private bool mRandomOrder;

		/// <summary>
		/// An object that can be used as a shared memory by the agents.
		/// </summary>
		private readonly Dictionary<string, dynamic> mMemory = new Dictionary<string, dynamic>();

		/// <summary>
		/// The agent collection
		/// </summary>
		private readonly AgentCollection mAgents;

		/// <summary>
		/// The agents in the environment
		/// </summary>
		public ICollection<Agent> Agents { get { return mAgents.Values; } }

		/// <summary>
		/// The number of agents in the environment
		/// </summary>
		public int AgentsCount { get { return mAgents.Count; } }

		/// <summary>
		/// Returns a list with the names of all the agents.
		/// </summary>
		public List<string> AgentsName { get { return mAgents.Keys.ToList(); } }

		/// <summary>
		/// Get element by name
		/// </summary>
		public Agent this[string name] { get { return mAgents[name]; } set { mAgents[name] = value; } }

		/// <summary>
		/// Initializes a new instance of the EnvironmentMas class.
		/// </summary>
		/// <param name="pDelayAfterTurn">A delay (in miliseconds) after each turn.</param>
		/// <param name="pRandomOrder">Whether the agents should be run in a random order (different each turn) or sequentially. If the execution is parallel, agents are always run in random order.</param>
		/// <param name="pRand">A random number generator for non-deterministic but repeatable experiments. It should instantiated using a seed. If it is null, a new Random object is created and used.</param>
		/// <param name="pParallel">Whether agent behaviors are executed in parallel or sequentially. The code of a single agent in a turn is always executed sequentially.</param>
		public EnvironmentMas(int pDelayAfterTurn = 0, bool pRandomOrder = true, Random pRand = null, bool pParallel = true) {
			Debug.Assert(mParallel && !mRandomOrder, "Sequential order cannot be guaranteed when executing in parallel. You can choose (randomOrder, parallel) to be (true, true), (false, false) or (true, false) (EnvironmentMas.ctor)");

			// Set attributs
			mDelayAfterTurn = pDelayAfterTurn;
			mRandomOrder = pRandomOrder;
			if (pRand == null) {
				mRand = new Random();
			} else {
				mRand = pRand;
			}
			mParallel = pParallel;
			mAgents = new AgentCollection(mParallel);
		}

		/// <summary>
		/// Returns the memory data
		/// </summary>
		/// <param name="pName">The name of the memory</param>
		/// <returns>The data</returns>
		public dynamic GetMemory(string pName) {
			return mMemory[pName];
		}

		/// <summary>
		/// Set the memory
		/// </summary>
		/// <param name="pName">The name of the memory</param>
		/// <param name="pData">The data</param>
		public void SetMemory(string pName, dynamic pData) {
			mMemory[pName] = pData;
		}

		/// <summary>
		/// Returns a list with the names of all the agents that contain a certain string.
		/// </summary>
		/// <param name="pNameFragment">The name fragment that the agent names should contain</param>
		/// <returns>The list of filtred agents</returns>
		public List<string> FilteredAgents(string pNameFragment) => AgentsName.FindAll(pName => pName.Contains(pNameFragment));

		/// <summary>
		/// Stops the execution of the agent identified by name and removes it from the environment. Use the Remove method instead of Agent.Stop
		/// when the decision to stop an agent does not belong to the agent itself, but to some other agent or to an external factor.
		/// </summary>
		/// <param name="pAgentName">The name of the agent to be removed</param>
		public void Remove(string pAgentName) => Remove(mAgents[pAgentName]);

		/// <summary>
		/// Adds an agent to the environment. Its name should be unique.
		/// </summary>
		/// <param name="pAgent">The concurrent agent that will be added</param>
		/// <param name="pName">The name of the agent</param>
		public void Add(Agent pAgent, string pName) {
			Debug.Assert(pName == null || pName == "", "Trying to add an agent without a name (EnvironmentMas.Add(agent))");
			Debug.Assert(mAgents.ContainsKey(pName), $"Trying to add an agent with an existing name: {pAgent.Name} (EnvironmentMas.Add(Agent))");

			pAgent.Name = pName;
			pAgent.Environment = this;
			mAgents[pAgent.Name] = pAgent;
		}

		/// <summary>
		/// Returns the name of a randomly selected agent from the environment
		/// </summary>
		/// <returns>Random agent</returns>
		public string RandomAgent() {
			int randomIndex = mRand.Next(mAgents.Count);
			var item = mAgents[randomIndex];
			return item.Key;
		}

		/// <summary>
		/// Stops the execution of the agent and removes it from the environment. Use the Remove method instead of Agent.Stop
		/// when the decision to stop an agent does not belong to the agent itself, but to some other agent or to an external factor.
		/// </summary>
		/// <param name="pAgent">The agent to be removed</param>
		public void Remove(Agent pAgent) {
			Debug.Assert(!mAgents.Values.Contains(pAgent), $"Agent {pAgent.Name} does not exist (Agent.Remove)");
			mAgents.Remove(pAgent.Name);
		}

		/// <summary>
		/// Sends a message from the outside of the multiagent system. Whenever possible, the agents should use the Send method of their own class,
		/// not the Send method of the environment. This method can also be used to simulate a forwarding behavior.
		/// </summary>
		/// <param name="pMessage">The message to be sent</param>
		public void Send(Message pMessage) {
			string receiverName = pMessage.Receiver;
			if (mAgents.ContainsKey(receiverName)) {
				mAgents[receiverName].Post(pMessage);
			}
		}

		/// <summary>
		/// Sort random array, faster than Enumerable.Range(0, n).ToArray();
		/// Fisher-Yates shuffle
		/// </summary>
		/// <param name="pNumber">The number of elements</param>
		/// <returns>Random permuted array of int</returns>
		public int[] RandomArray(int pNumber) {
			int[] lNumbers = new int[pNumber];
			for (int i = 0; i < pNumber; i++) {
				lNumbers[i] = i;
			}
			while (pNumber > 1) {
				int lIndex = mRand.Next(pNumber--);
				int lTemp = lNumbers[pNumber]; lNumbers[pNumber] = lNumbers[lIndex]; lNumbers[lIndex] = lTemp;
			}
			return lNumbers;
		}

		/// <summary>
		/// Sort array, faster than Enumerable.Range(0, n).ToArray();
		/// </summary>
		/// <param name="pNumber">The number of elements</param>
		/// <returns>Sorted array of int for 0 to pNumber - 1</returns>
		public int[] SortedArray(int pNumber) {
			int[] lNumbers = new int[pNumber];
			for (int i = 0; i < pNumber; i++) {
				lNumbers[i] = i;
			}
			return lNumbers;
		}

		/// **************************************************************** ///
		/// *************************** INTERNAL *************************** ///
		/// **************************************************************** ///

		/// <summary>
		/// Get the list of observable agents
		/// </summary>
		/// <param name="pPerceivingAgentName">The perceiving agent name</param>
		/// <param name="pPerceptionFilter">The perception filter function</param>
		/// <returns>List of observable agents</returns>>
		internal List<ObservableAgent> GetListOfObservableAgents(string pPerceivingAgentName, Func<Dictionary<string, string>, bool> pPerceptionFilter) {
			lock (mLocker) {
				var lObservableAgentList = new List<ObservableAgent>();
				var lAgentsNames = mAgents.Keys.ToList();

				for (int i = 0; i < lAgentsNames.Count; i++) {
					if (mAgents.Keys.Contains(lAgentsNames[i])) {
						Agent lAgent = mAgents[lAgentsNames[i]];

						if (lAgent == null || lAgent.Name == pPerceivingAgentName || lAgent.Observables == null || lAgent.Observables.Count == 0) {
							continue;
						}

						if (pPerceptionFilter(lAgent.Observables)) {
							lObservableAgentList.Add(new ObservableAgent(lAgent.Observables));
						}
					}
				}

				return lObservableAgentList;
			}
		}

		/// **************************************************************** ///
		/// *************************** PRIVATE **************************** ///
		/// **************************************************************** ///

		/// <summary>
		/// Execute perception and and action
		/// </summary>
		/// <param name="pAgent">The agent to execute</param>
		private void ExecutePerceptionDecisionAction(Agent pAgent) {
			if (pAgent.UsingObservables) {
				pAgent.InternalPerception();
			}
			pAgent.InternalAction();
		}

		/// <summary>
		/// Execute init
		/// </summary>
		/// <param name="pAgent">The agent to init</param>
		private void ExecuteSetup(Agent pAgent) {
			pAgent.Init();
			pAgent.IsInit = true;
		}

		/// **************************************************************** ///
		/// *************************** PROTECTED ************************** ///
		/// **************************************************************** ///

		/// <summary>
		/// Do one simulation turn
		/// </summary>
		/// <param name="pTurn">The current turn</param>
		protected void RunTurn(int pTurn) {
			if (!mParallel) {
				var lAgentOrder = mRandomOrder ? RandomArray(AgentsCount) : SortedArray(AgentsCount);
				var lAgentsLeft = new List<string>();
				var lAgentNames = mAgents.Keys.ToArray();

				for (int i = 0; i < AgentsCount; i++) {
					lAgentsLeft.Add(lAgentNames[lAgentOrder[i]]);
				}

				while (lAgentsLeft.Count > 0) {
					string lAgentLeft = lAgentsLeft[0];
					lAgentsLeft.Remove(lAgentLeft);

					if (mAgents.ContainsKey(lAgentLeft)) // agent not stopped or removed
					{
						if (!mAgents[lAgentLeft].IsInit) { // first turn runs Setup
							ExecuteSetup(mAgents[lAgentLeft]);
						} else {
							ExecutePerceptionDecisionAction(mAgents[lAgentLeft]);
						}
					}
				}
			} else {
				var lActions = new List<Action>();
				var lAgentsLeft = mAgents.Keys.ToList();

				while (lAgentsLeft.Count > 0) {
					string lAgentLeft = lAgentsLeft[0];
					lAgentsLeft.Remove(lAgentLeft);

					if (mAgents.ContainsKey(lAgentLeft)) // agent not stopped or removed
					{
						if (!mAgents[lAgentLeft].IsInit) { // first turn runs Setup
							lActions.Add(() => ExecuteSetup(mAgents[lAgentLeft]));
						} else {
							lActions.Add(() => ExecutePerceptionDecisionAction(mAgents[lAgentLeft]));
						}
					}
				}

				var lAction = new Action[lActions.Count];
				for (int i = 0; i < lActions.Count; i++) { // faster than .ToArray()
					lAction[i] = lActions[i];
				}
				Parallel.Invoke(lAction);
			}

			Thread.Sleep(mDelayAfterTurn);

			TurnFinished(pTurn);
		}

		/// **************************************************************** ///
		/// *************************** ABSTRACT *************************** ///
		/// **************************************************************** ///

		/// <summary>
		/// A method that may be optionally overriden to perform additional processing after the simulation has finished.
		/// </summary>
		public abstract void SimulationFinished();

		/// <summary>
		/// A method that may be optionally overriden to perform additional processing after a turn of the simulation has finished.
		/// </summary>
		/// <param name="pTurn">The turn that has just finished</param>
		public abstract void TurnFinished(int pTurn);
	}
}
