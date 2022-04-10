using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActressMasWrapper {

    // The graphic agent behaviour
    public abstract class EnvironmentBehaviour : MonoBehaviour {

        // ActressMas agent
        public EnvironmentWrapper Environment { get; protected set; }

        // Agent data in order to instanciated new agents
        protected Dictionary<string, AgentDataWrapper> mAgentDataWrapper = new();

        // Start is called before the first frame update
        protected void Init() {
            InitEnvironment();
        }

        // Update is called once per frame
        protected void RunTurn() {
            Environment.RunTurn(Time.deltaTime);
            CheckNewData();
        }

        // Add data to create
        public bool AddData(AgentDataWrapper mDataToWrap) {
            try {
                mAgentDataWrapper[mDataToWrap.Name] = mDataToWrap;
                return true;
            } catch (ArgumentException) {
                return false;
            }
        }

        // Get data to create
        public AgentDataWrapper GetData(string pName) {
            return mAgentDataWrapper[pName];
        }

        // In order to check if there is new agent to instanciated
        protected abstract void CheckNewData();

        // Create environment, prefab instancing etc.
        protected abstract void InitEnvironment();
    }
}
