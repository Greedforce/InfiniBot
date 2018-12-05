using System;

using Discord;

namespace InfiniBot
{
    public class ReactionGUIContainer
    {
        public ulong messageId;
        public Emote emote;
        public Action action;

        public ReactionGUIContainer(ulong messageId, Emote emote, Action action)
        {
            this.messageId = messageId;
            this.emote = emote;
            this.action = action;
        }

        public override bool Equals(object obj)
        {
            if (obj is ReactionGUIContainer)
            {
                return messageId == (obj as ReactionGUIContainer).messageId && emote.Name == (obj as ReactionGUIContainer).emote.Name;
            }
            return base.Equals(obj);
        }
    }
}
