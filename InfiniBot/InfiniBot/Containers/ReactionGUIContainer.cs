using System;
using System.Collections.Generic;

using Discord;

namespace InfiniBot
{
    public class ReactionGUIContainer
    {
        public ulong messageId;
        public Emoji emoji;
        public Action action;
        List<object> values = new List<object>();

        public ReactionGUIContainer(ulong messageId, Emoji emoji, Action action)
        {
            this.messageId = messageId;
            this.emoji = emoji;
            this.action = action;
        }

        public override bool Equals(object obj)
        {
            if (obj is ReactionGUIContainer)
            {
                return messageId == (obj as ReactionGUIContainer).messageId && emoji.Name == (obj as ReactionGUIContainer).emoji.Name;
            }
            return base.Equals(obj);
        }
    }
}
