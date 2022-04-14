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

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ActressMas {

	/// <summary>
	/// The base class for an agent that runs on a turn-based manner in its environment. You must create your own agent classes derived from this abstract class.
	/// </summary>
	public abstract class Agent {

		/// **************************************************************** ///
		/// *************************** ATTRIBUT *************************** ///
		/// **************************************************************** ///

		/// <summary>
		/// The environment in which the agent runs
		/// </summary>
		private readonly ConcurrentQueue<Message> pMessages = new ConcurrentQueue<Message>();

		/// <summary>
		/// If true the agent is init
		/// </summary>
		public bool IsInit { get; internal set; } = false;

		/// <summary>
		/// The properties of an agent which can be visible from the outside, i.e. perceivable by other agents
		/// </summary>
		public Dictionary<string, string> Observables { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Whether the agent uses the observable feature. The default value is false and it must be explicitly set to true before using observables
		/// </summary>
		public bool UsingObservables { get; set; } = false;

		/// <summary>
		/// The name of the agent. Each agent must have a unique name in its environment. Most operations are performed using agent names rather than agent objects
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// The environment in which the agent runs
		/// </summary>
		public EnvironmentMas Environment { get; internal set; }

		/// **************************************************************** ///
		/// **************************** PUBLIC **************************** ///
		/// **************************************************************** ///

		/// <summary>
		/// Constructor
		/// </summary>
		public Agent(string pName = "", EnvironmentMas pEnvironment = null) {
			Name = pName;
			Environment = pEnvironment;
		}

		/// <summary>
		/// Sends a message to all the agents in the environment
		/// </summary>
		/// <param name="pContent">The content of the message</param>
		/// <param name="pIncludeSender">Whether the sender itself receives the message or not</param>
		/// <param name="pConversationId">A conversation identifier, for the cases when a conversation involves multiple messages that refer to the same topic</param>
		public void Broadcast(string pContent, bool pIncludeSender = false, string pConversationId = "") {
			List<string> lReceivers = Environment.AgentsName;

			if (pIncludeSender == false) {
				lReceivers.Remove(Name);
			}
			foreach (string lAgent in lReceivers) {
				Send(lAgent, pContent, pConversationId);
			}
		}

		/// <summary>
		/// Sends a message to a specific agent, identified by name
		/// </summary>
		/// <param name="pReceiver">The agent that will receive the message. If the agent is in another container, use: agent@container</param>
		/// <param name="pContent">The content of the message</param>
		/// <param name="pConversationId">A conversation identifier, for the cases when a conversation involves multiple messages that refer to the same topic</param>
		public void Send(string pReceiver, string pContent, string pConversationId = "") {
			var message = new Message(Name, pReceiver, pContent, pConversationId);
			Environment.Send(message);
		}

		/// <summary>
		/// Sends a message to a specific set of agents, identified by name
		/// </summary>
		/// <param name="pReceivers">The list of agents that will receive the message</param>
		/// <param name="pContent">The content of the message</param>
		/// <param name="pConversationId">A conversation identifier, for the cases when a conversation involves multiple messages that refer to the same topic</param>
		public void SendToMany(List<string> pReceivers, string pContent, string pConversationId = "") {
			foreach (string lAgent in pReceivers) {
				Send(lAgent, pContent, pConversationId);
			}
		}

		/// <summary>
		/// Stops the execution of the agent and removes it from the environment. Use the Stop method instead of Environment
		/// Remove when the decision to be stopped belongs to the agent itself
		/// </summary>
		public void Stop() => Environment.Remove(this);

		/// **************************************************************** ///
		/// *************************** INTERNAL *************************** ///
		/// **************************************************************** ///

		/// <summary>
		/// Internal update
		/// </summary>
		internal virtual void InternalAction() {
			if (pMessages.Count > 0) {
				while (pMessages.Count > 0) {
					bool lOk = pMessages.TryDequeue(out Message lMessage);
					if (lOk) {
						Reaction(lMessage);
					}
				}
			} else {
				Action();
			}
		}

		/// <summary>
		/// Internal perception
		/// </summary>
		internal virtual void InternalPerception() => Perception(Environment.GetListOfObservableAgents(Name, PerceptionFilter));

		/// <summary>
		/// Internal post a message
		/// </summary>
		/// <param name="pMessage">The message that the agent to send</param>
		internal void Post(Message pMessage) => pMessages.Enqueue(pMessage);

		/// **************************************************************** ///
		/// *************************** ABSTRACT *************************** ///
		/// **************************************************************** ///

		/// <summary>
		/// This method is called as the first turn or right after an agent has moved to a new container
		/// It is similar to the constructor of the class, but it may be used for agent-related logic, e.g. for sending initial message(s)
		/// </summary>
		public abstract void Init();

		/// <summary>
		/// The function that identifies which properties and conditions must be satisfied by the Observables of other agents
		/// in order to be perceived by the observing agent. It must return true for the observables that will be available to the agent
		/// </summary>
		/// <param name="pObserved">A dictionary with name-value pairs of observed properties</param>
		/// <returns>True for the observables that will be available to the agent</returns>
		public abstract bool PerceptionFilter(Dictionary<string, string> pObserved);

		/// <summary>
		/// This method provides the agents whose observable properties are visible. It is called once a turn, before Update
		/// </summary>
		/// <param name="pObservableAgents">The list of agents which have at least one observable property desired by the observing agent. The desired properties are also available, from the ObservableAgent objects</param>
		public abstract void Perception(List<ObservableAgent> pObservableAgents);

		/// <summary>
		/// This is the method that is called when the agent does not receive any messages at the end of a turn
		/// </summary>
		public abstract void Action();

		/// <summary>
		/// This is the method that is called when the agent receives a message and is activated. This is where the main logic of the agent should be placed
		/// </summary>
		/// <param name="pMessage">The message that the agent has received and should respond to</param>
		public abstract void Reaction(Message pMessage);
	}
}
