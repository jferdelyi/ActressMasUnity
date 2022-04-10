using ActressMas;
using UnityEngine;

namespace ActressMasWrapper {

    // The agent wrapper
    public abstract class AgentWrapper : Agent {

        // The GameObject behaviour
        public AgentBehaviour Behavior { get; private set; }

        // Get transform
        public Transform Transform { get { return Behavior.transform; } }

        // The constructor
        public AgentWrapper(AgentBehaviour pBehaviour, EnvironmentWrapper pEnvironment) {
            Behavior = pBehaviour;
            Environment = pEnvironment;
        }

        // This is the method that is called once a turn
        public override void ActDefault() {
            Update();
        }

        // Must be define
        public abstract void Update();
    }
}