/**************************************************************************
 *                                                                        *
 *  Description: ActressMas multi-agent framework                         *
 *  Website:     https://github.com/florinleon/ActressMas                 *
 *  Copyright:   (c) 2018, Florin Leon                                    *
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

namespace ActressMas {

    /// <summary>
    /// A message that the agents use to communicate. In an agent-based system, the communication between the agents is exclusively performed by exchanging messages.
    /// </summary>
    [Serializable]
    public class Message {

        /// **************************************************************** ///
        /// *************************** ATTRIBUT *************************** ///
        /// **************************************************************** ///

        /// <summary>
        /// The action
        /// </summary>
        public string Action { get; protected set; }

        /// <summary>
        /// Parameters of the action
        /// </summary>
        public List<string> Parameters { get; private set; }

        /// <summary>
        /// The conversation identifier, for the cases when a conversation involves multiple messages that refer to the same topic
        /// </summary>
        public string ConversationId { get; private set; }

        /// <summary>
        /// The name of the agent that needs to receive the message
        /// </summary>
        public string Receiver { get; private set; }

        /// <summary>
        /// The name of the agent that sends the message
        /// </summary>
        public string Sender { get; private set; }

        /// **************************************************************** ///
        /// **************************** PUBLIC **************************** ///
        /// **************************************************************** ///

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="pSender">The name of the agent that sends the message</param>
        /// <param name="pReceiver">The name of the agent that needs to receive the message</param>
        /// <param name="pContent">The content of the message</param>
        /// <param name="pConversationId">The conversation identifier, for the cases when a conversation involves multiple messages that refer to the same topic</param>
        public Message(string pSender, dynamic pReceiver, string pContent, string pConversationId) {
            Sender = pSender;
            Receiver = pReceiver;
            Parse(pContent, out string lAction, out List<string> lParameters);
            Action = lAction;
            Parameters = lParameters;
            ConversationId = pConversationId;
        }

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="pSender">The name of the agent that sends the message</param>
        /// <param name="pReceiver">The name of the agent that needs to receive the message</param>
        /// <param name="pAction">The action</param>
        /// <param name="pParameters">Parameters</param>
        /// <param name="pConversationId">The conversation identifier, for the cases when a conversation involves multiple messages that refer to the same topic</param>
        public Message(string pSender, dynamic pReceiver, string pAction, List<string> pParameters, string pConversationId) {
            Sender = pSender;
            Receiver = pReceiver;
            Action = pAction;
            Parameters = pParameters;
            ConversationId = pConversationId;
        }

        /// <summary>
        /// Returns a string of the form "[Sender -> Receiver]: Content"
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString() => $"[{Sender} -> {Receiver}] {Action}:[{string.Join(", ", Parameters)}]";

        /// **************************************************************** ///
        /// ************************** PROTECTED *************************** ///
        /// **************************************************************** ///

        /// <summary>
        /// Parses the content of a message and identifies the action (similar, e.g., to a performative) and the list of parameters
        /// <param name="pContent">The concat string</param>
        /// <param name="pAction">The action</param>
        /// <param name="pParameters">Parameters</param>
        /// </summary>
        protected void Parse(string pContent, out string pAction, out List<string> pParameters) {
            string[] lSplitedContent = pContent.Split();
            pAction = lSplitedContent[0];
            pParameters = new List<string>();
            for (int i = 1; i < lSplitedContent.Length; i++) {
                pParameters.Add(lSplitedContent[i]);
            }
        }
    }
}
