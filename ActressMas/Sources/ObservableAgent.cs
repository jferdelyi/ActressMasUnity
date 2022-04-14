/**************************************************************************
 *                                                                        *
 *  Description: ActressMas multi-agent framework                         *
 *  Website:     https://github.com/florinleon/ActressMas                 *
 *  Copyright:   (c) 2020, Florin Leon                                    *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using System.Collections.Generic;

namespace ActressMas {

    /// <summary>
    /// The class that represents the observable properties of an agent. They depend on the set of Observables properties of an agent and
    /// on the PerceptionFilter function of an agent who wants to observe other agents.
    /// </summary>
    public class ObservableAgent {

        /// <summary>
        /// The properties of the observed agent which are visible to the agent who registers to see them.
        /// They are a subset of the full Observables properties of an agent.
        /// </summary>
        public Dictionary<string, string> Observed { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the ObservableAgent class.
        /// </summary>
        /// <param name="pObservable">A collection of observable properties</param>
        public ObservableAgent(Dictionary<string, string> pObservable) {
            Observed = new Dictionary<string, string>(pObservable);
        }
    }
}
