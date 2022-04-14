using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActressMas {

    /// <summary>
    /// Collection of agent
    /// </summary>
    internal class AgentCollection {

        /// **************************************************************** ///
        /// *************************** ATTRIBUT *************************** ///
        /// **************************************************************** ///

        /// <summary>
        /// If true use concurent dictionary
        /// </summary>
        protected bool mParallel;

        /// <summary>
        /// The dictionary
        /// </summary>
        protected IDictionary<string, Agent> mAgents;

        /// <summary>
        /// Keys
        /// </summary>
        public ICollection<string> Keys { get { return mAgents.Keys; } }

        /// <summary>
        /// Values
        /// </summary>
        public ICollection<Agent> Values { get { return mAgents.Values; } }

        /// <summary>
        /// Count
        /// </summary>
        public int Count { get { return mAgents.Count; } }

        /// <summary>
        /// Get element by index
        /// </summary>
        public KeyValuePair<string, Agent> this[int index] { get { return mAgents.ElementAt(index); } }

        /// <summary>
        /// Get element by name
        /// </summary>
        public Agent this[string name] { get {
                try {
                    return mAgents[name];
                } catch (KeyNotFoundException) {
                    return null;
                }
            } set { mAgents[name] = value; } }

        /// **************************************************************** ///
        /// **************************** PUBLIC **************************** ///
        /// **************************************************************** ///

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pParallel">If true use concurent dictionary</param>
        public AgentCollection(bool pParallel) {
            mParallel = pParallel;
            if (pParallel) {
                mAgents = new ConcurrentDictionary<string, Agent>();
            } else {
                mAgents = new Dictionary<string, Agent>();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pName">If true use concurent dictionary</param>
        public void Remove(string pName) {
            if (mParallel) {
                bool lOk = ((ConcurrentDictionary<string, Agent>)mAgents).TryRemove(pName, out _);
                Debug.Assert(!lOk, $"Agent {pName} could not be removed (EnvironmentMas.Remove(string) -> AgentCollection(parallel).Remove)");
            } else {
                mAgents.Remove(pName);
            }
        }

        /// <summary>
        /// Contains key wrapping
        /// </summary>
        public bool ContainsKey(string name) => mAgents.ContainsKey(name);
    }
}
