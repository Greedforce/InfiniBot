using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace InfiniBot
{
    public static class EmbedBuilderExtensionMethods
    {
        public static EmbedBuilder WithAutoDeletionFooter(this EmbedBuilder embedBuilder)
        {
            return embedBuilder
                .WithFooter($"(This message will delete itself in {Data.MESSAGE_DELETE_DELAY} seconds)");
        }
    }
}
