using ActressMas;
using UnityEngine;

namespace ActressMasWrapper {

    // The environment wrapper
    public abstract class EnvironmentWrapper : EnvironmentMas {

        // Number of turns
        public int Turn { get; protected set; }

        // Sum time
        public double TotalTime { get; protected set; }

        // The GameObject behaviour
        public EnvironmentBehaviour Behavior { get; protected set; }

        // Simulation per seconds
        public int SimulationPerSeconds { get; protected set; }

        // Get transform
        public Transform Transform { get { return Behavior.transform; } }

        // Constructor
        public EnvironmentWrapper(int pSimulationPerSeconds, EnvironmentBehaviour pBehaviour) : base(rand: Tools.Rand) {
            SimulationPerSeconds = pSimulationPerSeconds;
            Behavior = pBehaviour;
            Turn = 0;
        }

        // Update simulation
        public void RunTurn(double pDeltaTime) {
            TotalTime += pDeltaTime;

            if (TotalTime >= (1 / (double)SimulationPerSeconds)) {
                TotalTime %= (1 / (double)SimulationPerSeconds);
                base.RunTurn(Turn);
            }

            Turn++;
        }

        // Perform additional processing after the simulation has finished
        void OnDestroy() {
            SimulationFinished();
        }

        // Perform additional processing after a turn of the the simulation has finished
        public override void TurnFinished(int pTurn) {
            TurnEnd(pTurn);
        }

        // Redefinition from ActressMas
        public new abstract void SimulationFinished();

        // Must be define
        public abstract void TurnEnd(int pTurn);
    }
}