using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace InfiniBot
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Shutdown", RunMode = RunMode.Sync)]
        [Summary("Shuts down the bot and provides the reason, if given.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task ShutdownAsync(
            [Summary("The reason for the shutdown.")]
            [Example("Adding new feature")]
            [Remainder]
            string reason = "Non given.")
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(embed: new EmbedBuilder().WithColor(Data.COLOR_BOT).WithTitle("Shutting down").WithDescription($"Reason: {reason}").Build());
            Thread.Sleep(1000);
            await Program.MainForm.bot.StopBotAsync();
        }

        [Command("Say", RunMode = RunMode.Async)]
        [Summary("Causes the bot to send the given message.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task SayAsync(
            [Summary("The message you wish the bot to send.")]
            [Example("Hi I'm a bot. :D")]
            [Remainder]
            string message)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(message);
        }

        [Command("JoinMessage", RunMode = RunMode.Async)]
        [Summary("Causes the bot to send the join message to the specified user.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task ResendJoinMessage(
            [Summary("The user who will receive the message")]
            [Example("Hi I'm a bot. :D")]
            SocketUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null)
            {
                user = Context.User;
            }

            Embed embed = Data.GetJoinEmbed(Context.Guild);
            await user.SendMessageAsync("", false, embed);
            IMessage m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithTitle("Join Message Sent").WithDescription($"I've PMed the join message to {user.Username}").Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }
    }
}
