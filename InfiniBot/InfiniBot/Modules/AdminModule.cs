using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace InfiniBot
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("Shutdown", RunMode = RunMode.Sync)]
        [Summary("Shuts down the bot and provides the reason, if given.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task ShutdownAsync(
            [Summary("The reason for the shutdown.")]
            [Example("Adding new feature")]
            [Remainder]
            string reason = "None given.")
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
            [Summary("The user who will receive the message. If left empty, this defaults to the command user.")]
            [Example("@InfiniBot#6309")]
            [Example("238279899687813120")]
            SocketUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null)
            {
                user = Context.User;
            }

            Embed embed = Data.GetJoinEmbed(Context.Guild);
            await user.SendMessageAsync(embed: embed);
            IMessage m = await ReplyAsync(
                embed: new EmbedBuilder()
                .WithTitle("Join Message Sent")
                .WithDescription($"I've PMed the join message to {user.Username}")
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("RolePermissions", RunMode = RunMode.Async)]
        [Summary("Prompts the bot to display every role on the server and their permissions.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task GetRolePermissionsAsync()
        {
            // Remake properly with embeds and channel overwrite permissions, using code below

            /*// Get all Overwrites for the current SocketRole (sr)
            //Context.Guild.Channels.Where(c => c.GetPermissionOverwrite(sr).HasValue).ToList().ForEach(c => roleOverwrites.AddRange(c.PermissionOverwrites.Where(o => o.TargetType == PermissionTarget.Role && o.TargetId == sr.Id)));
            List<Overwrite> roleOverwrites = new List<Overwrite>();
            foreach (SocketGuildChannel c in channels)
            {
                List<Overwrite> channelOverwrites = c.PermissionOverwrites.ToList();
                foreach (Overwrite o in channelOverwrites)
                {
                    if (o.TargetType == PermissionTarget.Role &&
                        o.TargetId == sr.Id)
                    {
                        roleOverwrites.Add(o);
                    }
                }
            }*/

            string toReturn = "";
            foreach (SocketRole sr in Context.Guild.Roles)
            {
                List<GuildPermission> privs = sr.Permissions.ToList();
                string privStr = "";
                foreach (GuildPermission p in privs)
                {
                    privStr += "\n" + p.ToString();
                }
                string newStr = "**" + sr.Name + "**, permissions(" + privs.Count + "):" + privStr + "\n";
                if ((toReturn + newStr).Length > Data.CHAR_LIMIT)
                {
                    await ReplyAsync(toReturn);
                    toReturn = newStr;
                }
                else
                {
                    toReturn += newStr;
                }
            }
            await ReplyAsync(toReturn);
        }
    }
}
