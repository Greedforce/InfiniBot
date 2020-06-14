using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace InfiniBot
{
    public class WarningModule : ModuleBase<SocketCommandContext>
    {
        [Command("Warn", RunMode = RunMode.Async)]
        [Alias("Warning", "Strike")]
        [Summary("Warns the user a list of all available roles")]
        public async Task WarnUserAsync(
            [Summary("The user who will receive the warning.")]
            [Example("@InfiniBot#6309")]
            [Example("238279899687813120")]
            SocketUser user,
            [Summary("The reason for the warning.")]
            [Example("Posted racist and/or hopophobic content.")]
            string reason)
        {

        }
    }
}
