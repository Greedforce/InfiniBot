using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace InfiniBot
{
    public class HelpfulCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Help", RunMode = RunMode.Async)]
        [Summary("Provides information on a specific command or module, or a list of all available commands and modules, if no parameter is provided.")]
        public async Task SendHelpAsync([Summary("The name of a command or module")][Remainder]string commandOrModule = "")
        {
            EmbedBuilder builder = new EmbedBuilder();
            string
                toReturn = "",
                title = "";

            if (commandOrModule == "")
            {
                #region Command and Module List
                title = "Help - List";

                List<CommandInfo> commandInfos = Program.MainForm.bot.commands.Commands.Where(c => c.Module.Group == null).ToList();
                for (int i = 0; i < commandInfos.Count(); i++)
                {
                    if ((await commandInfos[i].CheckPreconditionsAsync(Context)).IsSuccess &&
                        commandInfos[i].Attributes.FirstOrDefault(a => a is HideAttribute) == null)
                    {
                        if (i > 0)
                        {
                            toReturn += ", ";
                        }
                        toReturn += '`' + commandInfos[i].Name + '`';
                    }
                }

                builder.AddField("Commands", toReturn);
                toReturn = "";

                List<ModuleInfo> moduleInfos = Program.MainForm.bot.commands.Modules.Where(m => m.Group != null).ToList();
                for (int i = 0; i < moduleInfos.Count(); i++)
                {
                    if (moduleInfos[i].Commands.FirstOrDefault(c => c.CheckPreconditionsAsync(Context).GetAwaiter().GetResult().IsSuccess) != null &&
                        moduleInfos[i].Attributes.FirstOrDefault(a => a is HideAttribute) == null)
                    {
                        /*if ((toReturn + "\n**" + m.Group + "**").Length > Data.CHAR_LIMIT)
                        {
                            await Context.User.SendMessageAsync(toReturn);
                            toReturn += "**" + m.Group + "**";
                        }
                        else
                        {
                            toReturn += "\n**" + m.Group + "**";
                        }*/
                        if (i > 0)
                        {
                            toReturn += ", ";
                        }
                        toReturn += '`' + moduleInfos[i].Group + '`';

                        /*foreach (var c in m.Commands)
                        {
                            if ((await c.CheckPreconditionsAsync(Context)).IsSuccess &&
                                c.Attributes.FirstOrDefault(a => a is HideAttribute) == null)
                            {
                                if ((toReturn + "\n-" + c.Name).Length > Data.CHAR_LIMIT)
                                {
                                    await Context.User.SendMessageAsync(toReturn);
                                    toReturn = "-" + c.Name;
                                }
                                else
                                {
                                    toReturn += "\n-" + c.Name;
                                }
                            }
                        }*/
                    }
                }
                await Context.User.SendMessageAsync("", embed: builder.WithColor(Data.COLOR_BOT).WithTitle(title).AddField("Modules*", toReturn).WithFooter("*Modules contain sub-commands.").Build());
                #endregion
            }
            else
            {
                ModuleInfo moduleInfo = Program.MainForm.bot.commands.Modules.Where(m =>
                m.Group != null).FirstOrDefault(m => m.Group.ToLower() == commandOrModule.ToLower() ||
                m.Aliases.FirstOrDefault(a => a.ToLower() == commandOrModule.ToLower()) != null);
                if (moduleInfo != null)
                {
                    #region Module Help
                    title = "Module Help - " + moduleInfo.Group;
                    toReturn += "\n**Module**: `" + moduleInfo.Group + "`";
                    toReturn += "\n**Aliases**: ";
                    if (moduleInfo.Aliases.Count > 1)
                    {
                        for (int i = 1; i < moduleInfo.Aliases.Count; i++)
                        {
                            if (i > 1)
                            {
                                toReturn += ", ";
                            }
                            toReturn += "`" + moduleInfo.Aliases[i].Split(' ').Last() + "`";
                        }
                    }
                    else
                    {
                        toReturn += "`None`";
                    }
                    toReturn += "\n**Description**\n`" + moduleInfo.Summary + "`";
                    toReturn += "\n**Permission required**: " + GetPreconditionsAsString(moduleInfo.Preconditions);
                    toReturn += "\n**Commands**:\n";
                    if (moduleInfo.Commands.Count > 0)
                    {
                        for (int i = 0; i < moduleInfo.Commands.Count; i++)
                        {
                            if (i > 0)
                            {
                                toReturn += ", ";
                            }
                            toReturn += "`" + moduleInfo.Commands[i].Name + "`";
                        }
                    }
                    else
                    {
                        toReturn += "`None`";
                    }
                    toReturn += "\n\n**Usage**:\n" + Data.BOT_PREFIX + moduleInfo.Group + " <command> (command parameters)";

                    await Context.User.SendMessageAsync("", embed: builder.WithColor(Data.COLOR_BOT).WithTitle(title).WithDescription(toReturn).WithFooter("<> = required parameter, () = depends on the command in question").Build());
                    #endregion
                }
                else
                {
                    CommandInfo commandInfo = Program.MainForm.bot.commands.Commands.FirstOrDefault(c =>
                    c.Name.ToLower() == commandOrModule.ToLower() ||
                    c.Aliases.FirstOrDefault(a => a.ToLower() == commandOrModule.ToLower()) != null);
                    if (commandInfo == null)
                    {
                        commandInfo = Program.MainForm.bot.commands.Commands.FirstOrDefault(c =>
                        (c.Module.Group + ' ' + c.Name).ToLower() == commandOrModule.ToLower() ||
                        c.Module.Aliases.FirstOrDefault(a => (a + ' ' + c.Name).ToLower() == commandOrModule.ToLower()) != null);
                        for (int i = 1; i < commandInfo.Aliases.Count; i++)
                        {
                            if (commandInfo == null)
                            {
                                commandInfo = Program.MainForm.bot.commands.Commands.FirstOrDefault(c =>
                                (c.Module.Group + ' ' + commandInfo.Aliases[i]).ToLower() == commandOrModule.ToLower() ||
                                c.Module.Aliases.FirstOrDefault(a => (a + ' ' + commandInfo.Aliases[i]).ToLower() == commandOrModule.ToLower()) != null);
                            }
                            else
                            {
                                i = commandInfo.Aliases.Count;
                            }
                        }
                    }

                    if (commandInfo != null)
                    {
                        #region Command Help
                        //toReturn = "**" + commandInfo.Name + "**";
                        //toReturn += "\n" + commandInfo.Summary;

                        //toReturn = commandInfo.Summary;

                        toReturn += "\n**Name**: `" + commandInfo.Name + "`";
                        toReturn += "\n**Aliases**: ";
                        if (commandInfo.Aliases.Count > 1)
                        {
                            for (int i = 1; i < commandInfo.Aliases.Count / commandInfo.Module.Aliases.Count; i++)
                            {
                                if (i > 1)
                                {
                                    toReturn += ", ";
                                }
                                toReturn += "`" + commandInfo.Aliases[i].Split(' ').Last() + "`";
                            }
                        }
                        else
                        {
                            toReturn += "`None`";
                        }
                        toReturn += "\n**Module**: ";
                        if (commandInfo.Module.Group != null)
                        {
                            toReturn += "`" + commandInfo.Module.Group + "`";
                            title = "Command Help - " + commandInfo.Module.Group + " " + commandInfo.Name;
                        }
                        else
                        {
                            toReturn += "`None`";
                            title = "Command Help - " + commandInfo.Name;
                        }
                        toReturn += "\n**Description**\n`" + commandInfo.Summary + "`";
                        toReturn += "\n**Permission required**: " + GetPreconditionsAsString(commandInfo.Preconditions);
                        toReturn += "\n**Parameters**: ";
                        string example = Data.BOT_PREFIX;
                        if (commandInfo.Module.Group != null)
                        {
                            example += commandInfo.Module.Group + ' ';
                        }
                        example += commandInfo.Name;
                        if (commandInfo.Parameters.Count > 0)
                        {
                            foreach (var p in commandInfo.Parameters)
                            {
                                toReturn += "\n`" + p.Name + "`";
                                if (p.IsOptional)
                                {
                                    toReturn += " (Optional)";

                                    example += " [" + p.Name + "]";
                                }
                                else
                                {
                                    example += " <" + p.Name + ">";
                                }
                                if (!string.IsNullOrEmpty(p.Summary) && !string.IsNullOrWhiteSpace(p.Summary))
                                {
                                    toReturn += " - " + p.Summary;
                                }
                            }
                        }
                        else
                        {
                            toReturn += "\n`None`";
                        }
                        toReturn += "\n\n**Usage**:\n" + example;
                        await Context.User.SendMessageAsync("", embed: builder.WithColor(Data.COLOR_BOT).WithTitle(title).WithDescription(toReturn).WithFooter("<> = required parameter, [] = optional parameter").Build());
                        #endregion
                    }
                    else
                    {
                        #region Non-existant Command or Module
                        toReturn = "`" + commandOrModule + "` is not a valid command or module. Please check your spelling and try again. (If you want a list of all available commands and modules, please use this command again, without any parameters.)";
                        await ReplyAsync(embed: builder.WithColor(Data.COLOR_ERROR).WithDescription(toReturn).WithTitle("Help - Error").Build());
                        #endregion
                    }
                }
            }
        }

        [Command("Roll", RunMode = RunMode.Async)]// Needs to be updated to a better system
        [Alias("R")]
        [Summary("Rolls the specified dice and displays the result.")]
        public async Task RollDice(
            [Summary("The dice you wish to roll, following the format of \"<nr of dice>d<sides of the dice>+modifier\"")]string input)
        {
            await Context.Message.DeleteAsync();
            string toReturn = "Roll: " + input + "\n";
            string[] arguments = input.Split(new string[] { "d" }, StringSplitOptions.None);
            string x = arguments[0];
            arguments = arguments[1].Split(new string[] { "+" }, StringSplitOptions.None);
            if (arguments.Length <= 1)
            {
                arguments = arguments[1].Split(new string[] { "-" }, StringSplitOptions.None);
            }
            string y = arguments[0];
            string z = "0";
            if (arguments.Length > 1)
            {
                z = arguments[1];
            }

            int total = 0;
            if (int.TryParse(x, out int nrOfDice) &&
            int.TryParse(y, out int sides) &&
            int.TryParse(z, out int extra))
            {
                int[] results = new int[nrOfDice];

                Random dice = new Random();
                for (int i = 0; i < nrOfDice; i++)
                {
                    results[i] = dice.Next(sides);
                    results[i]++;
                    total += results[i];
                    if (i != 0)
                    {
                        toReturn += " + ";
                    }
                    toReturn += $"[{results[i]}]";
                }
                if (extra > 0)
                {
                    total += extra;
                    toReturn += $" + {extra}";
                }
                else if (extra < 0)
                {
                    total -= extra;
                    toReturn += $" - {extra}";
                }
                toReturn += $" = {total}";
                await ReplyAsync(embed: new EmbedBuilder().WithTitle("Roll").WithDescription(toReturn).Build());
            }
            else
            {
                IUserMessage m = await ReplyAsync(embed: new EmbedBuilder().WithTitle("Roll - Error").WithDescription($"I cannot roll `{x}d{y}+{z}´").Build());
                Thread.Sleep(3000);
                await m.DeleteAsync();
            }
        }

        [Command("Group", RunMode = RunMode.Async)]
        [Alias("G")]
        [Summary("Takes the given objects and splits them into the specified number of groups.")]
        public async Task SplitMembersIntoGroups([Summary("The number of groups you wish to divide the objects into.")]int groups,
            [Summary("The objects you wish to divide, separated by spaces.")][Remainder]string objects)
        {
            await Context.Message.DeleteAsync();
            string toReturn = "";
            string[] b = objects.Split(' ');

            for (int i = 0; i < b.Length / 2; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    int x = Bot.rng.Next(b.Length);
                    string temp = b[j];
                    b[j] = b[x];
                    b[x] = temp;
                }
            }

            toReturn = $"I've randomly split the {b.Length} members into the following {groups} groups.";
            int remainder = b.Length % groups;
            int member = 0;
            for (int i = 0; i < groups; i++)
            {
                toReturn += "\n\nGroup " + (i + 1) + ":";
                for (int j = 0; j < b.Length / groups; j++)
                {
                    toReturn += $"\n{b[j + member]}";
                }
                member += (b.Length / groups);
                if (i < remainder)
                {
                    toReturn += $"\n{b[member]}";
                    member++;
                }
            }
            await ReplyAsync(embed: new EmbedBuilder().WithColor(Data.COLOR_BOT).WithTitle("Group").WithDescription(toReturn).Build());
        }

        // Takes a topic and the spoiler then shows the topic while keeping the spoiler hidden under a hover link
        [Command("Spoiler")]
        [Summary("A command to post spoilers safely")] // Example: !spoiler ""This is the topic"" ""This is the spoiler""
        public async Task Spoiler(
            [Summary("Topic the spoiler is about, (use \"\" for multiple words)")]
            [Example("Titanic")]
            [Example("\"My Hero Academia\"")]
            string spoilerTopic, 
            [Summary("The spoiler")]
            [Example("The boat sinks")]
            [Example("\"Izuku Midoriya is a fan of All might\"")]
            [Remainder]
            string spoiler)
        {
            EmbedBuilder builder = new EmbedBuilder();

            await Context.Message.DeleteAsync();

            builder.WithTitle(Context.User.Username + " posted a spoiler about: " + spoilerTopic);

            builder.WithDescription($"[Hover me for spoiler!](https://bit.ly/2Dznxn0 '{spoiler}')"); // Also it links to the wikipedia article about spoilers

            builder.WithColor(Data.COLOR_BOT);
            
            await ReplyAsync("", false, embed: builder.Build());
        }

        private string GetPreconditionsAsString(IReadOnlyList<PreconditionAttribute> preconditions)
        {
            List<Tuple<string, List<string>>> groups = new List<Tuple<string, List<string>>>();
            for (int i = 0; i < preconditions.Count; i++)
            {
                Tuple<string, List<string>> group = groups.FirstOrDefault(g => g.Item1 == preconditions[i].Group);
                if (group == null)
                {
                    groups.Add(new Tuple<string, List<string>>(preconditions[i].Group, new List<string>()));
                    group = groups.LastOrDefault();
                }

                if (preconditions[i] is RequireOwnerAttribute)
                {
                    group.Item2.Add("`Owner`");
                }
                else if (preconditions[i] is RequireUserPermissionAttribute)
                {
                    switch ((preconditions[i] as RequireUserPermissionAttribute).GuildPermission)
                    {
                        case GuildPermission.CreateInstantInvite:
                            group.Item2.Add("`Create Instant Invite`");
                            break;
                        case GuildPermission.KickMembers:
                            group.Item2.Add("`Kick Members`");
                            break;
                        case GuildPermission.BanMembers:
                            group.Item2.Add("`Ban Members`");
                            break;
                        case GuildPermission.Administrator:
                            group.Item2.Add("`Administrator`");
                            break;
                        case GuildPermission.ManageChannels:
                            group.Item2.Add("`Manage Channels`");
                            break;
                        case GuildPermission.ManageGuild:
                            group.Item2.Add("`Manage Server/Guild`");
                            break;
                        case GuildPermission.AddReactions:
                            group.Item2.Add("`Add Reactions`");
                            break;
                        case GuildPermission.ViewAuditLog:
                            group.Item2.Add("`View Audit Log`");
                            break;
                        case GuildPermission.ViewChannel:
                            group.Item2.Add("`View Channel`");
                            break;
                        case GuildPermission.SendMessages:
                            group.Item2.Add("`Send Messages`");
                            break;
                        case GuildPermission.SendTTSMessages:
                            group.Item2.Add("`Send TTS Messages`");
                            break;
                        case GuildPermission.ManageMessages:
                            group.Item2.Add("`Manage Messages`");
                            break;
                        case GuildPermission.EmbedLinks:
                            group.Item2.Add("`Embed Links`");
                            break;
                        case GuildPermission.AttachFiles:
                            group.Item2.Add("`Attach Files`");
                            break;
                        case GuildPermission.ReadMessageHistory:
                            group.Item2.Add("`Read Message History`");
                            break;
                        case GuildPermission.MentionEveryone:
                            group.Item2.Add("`Mention Everyone`");
                            break;
                        case GuildPermission.UseExternalEmojis:
                            group.Item2.Add("`Use External Emojis`");
                            break;
                        case GuildPermission.Connect:
                            group.Item2.Add("`Connect`");
                            break;
                        case GuildPermission.Speak:
                            group.Item2.Add("`Speak`");
                            break;
                        case GuildPermission.MuteMembers:
                            group.Item2.Add("`Mute Members`");
                            break;
                        case GuildPermission.DeafenMembers:
                            group.Item2.Add("`Deafen Members`");
                            break;
                        case GuildPermission.MoveMembers:
                            group.Item2.Add("`Move Members`");
                            break;
                        case GuildPermission.UseVAD:
                            group.Item2.Add("`Use Voice Activity`");
                            break;
                        case GuildPermission.ChangeNickname:
                            group.Item2.Add("`Change Nickname`");
                            break;
                        case GuildPermission.ManageNicknames:
                            group.Item2.Add("`Manage Nicknames`");
                            break;
                        case GuildPermission.ManageRoles:
                            group.Item2.Add("`Manage Roles`");
                            break;
                        case GuildPermission.ManageWebhooks:
                            group.Item2.Add("`Manage Webhooks`");
                            break;
                        case GuildPermission.ManageEmojis:
                            group.Item2.Add("`Manage Emojis`");
                            break;
                    }

                    switch ((preconditions[i] as RequireUserPermissionAttribute).ChannelPermission)
                    {
                        case ChannelPermission.CreateInstantInvite:
                            group.Item2.Add("`Create Instant Invite`");
                            break;
                        case ChannelPermission.ManageChannels:
                            group.Item2.Add("`Manage Channels`");
                            break;
                        case ChannelPermission.AddReactions:
                            group.Item2.Add("`Add Reactions`");
                            break;
                        //case ChannelPermission.ReadMessages:
                        case ChannelPermission.ViewChannel:
                            group.Item2.Add("`Read Messages`");
                            break;
                        case ChannelPermission.SendMessages:
                            group.Item2.Add("`Send Messages`");
                            break;
                        case ChannelPermission.SendTTSMessages:
                            group.Item2.Add("`Send TTS Messages`");
                            break;
                        case ChannelPermission.ManageMessages:
                            group.Item2.Add("`Manage Messages`");
                            break;
                        case ChannelPermission.EmbedLinks:
                            group.Item2.Add("`Embed Links`");
                            break;
                        case ChannelPermission.AttachFiles:
                            group.Item2.Add("`Attach Files`");
                            break;
                        case ChannelPermission.ReadMessageHistory:
                            group.Item2.Add("`Read Message History`");
                            break;
                        case ChannelPermission.MentionEveryone:
                            group.Item2.Add("`Mention Everyone`");
                            break;
                        case ChannelPermission.UseExternalEmojis:
                            group.Item2.Add("`Use External Emojis`");
                            break;
                        case ChannelPermission.Connect:
                            group.Item2.Add("`Connect`");
                            break;
                        case ChannelPermission.Speak:
                            group.Item2.Add("`Speak`");
                            break;
                        case ChannelPermission.MuteMembers:
                            group.Item2.Add("`Mute Members`");
                            break;
                        case ChannelPermission.DeafenMembers:
                            group.Item2.Add("`Deafen Members`");
                            break;
                        case ChannelPermission.MoveMembers:
                            group.Item2.Add("`Move Members`");
                            break;
                        case ChannelPermission.UseVAD:
                            group.Item2.Add("`Use Voice Activity`");
                            break;
                        case ChannelPermission.ManageRoles:
                            group.Item2.Add("`Manage Roles`");
                            break;
                        case ChannelPermission.ManageWebhooks:
                            group.Item2.Add("`Manage Webhooks`");
                            break;
                    }
                }
            }

            string toReturn = "";

            for (int i = 0; i < groups.Count; i++)
            {
                if (i > 0)
                {
                    toReturn += ", ";
                }
                if (groups[i].Item2.Count > 1)
                {
                    toReturn += "(";
                }
                for (int j = 0; j < groups[i].Item2.Count; j++)
                {
                    if (j > 0)
                    {
                        toReturn += " or ";
                    }
                    toReturn += $"`{groups[i].Item2[j]}`";
                }
                if (groups[i].Item2.Count > 1)
                {
                    toReturn += ")";
                }
            }

            if (string.IsNullOrEmpty(toReturn))
            {
                toReturn = "`None`";
            }

            return toReturn;
        }
    }
}
