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
        [Command("Shutdown")]
        [Summary("Shuts down the bot and provides the reason, if given.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        [Hide]
        public async Task ShutdownAsync([Summary("The reason for the shutdown.")][Remainder]string reason = "Non given.")
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(embed: new EmbedBuilder().WithColor(Data.COLOR_BOT).WithTitle("Shutting down").WithDescription($"Reason: {reason}").Build());
            Thread.Sleep(1000);
            await Program.MainForm.ToggleBot();
        }

        [Command("Say")]
        [Summary("Causes the bot to send the given message.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        [Hide]
        public async Task SayAsync([Summary("The message you wish the bot to send.")][Remainder]string message)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(message);
        }

        [Command("JoinMessage")]
        [Summary("Causes the bot to send the join message to the specified user.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        [Hide]
        public async Task ResendJoinMessage(SocketUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null)
            {
                user = Context.User;
            }

            Embed embed = Data.GetJoinEmbed(Context.Guild);
            await user.SendMessageAsync("", false, embed);
            IMessage m = await ReplyAsync(embed: Data.GetFeedbackEmbed().WithTitle("Join Message Sent").WithDescription($"I've PMed the join message to {user.Username}").Build());
            Data.tempMessages.Add(new TempMessage(m));
        }
    }
}
