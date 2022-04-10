using UnityEngine;

namespace ActressMasWrapper {

    // The graphic agent behaviour
    public abstract class AgentBehaviour : MonoBehaviour {

        // ActressMas agent
        public AgentWrapper Agent { get; protected set; }
    }
}
