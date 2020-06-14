using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace InfiniBot
{
    public class MessageManagementModule : ModuleBase<SocketCommandContext>
    {
        [Command("Prune", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        [Summary("Deletes the last x messages sent in the text channel.")]
        public async Task PruneMessagesAsync(
            [Summary("How many messages you want to delete.")]
            [Example("10")]
            int amount,
            [Summary("How many messages you want to skip before starting to delete.")]
            [Example("2")]
            int skip = 0)
        {
            await Context.Message.DeleteAsync();
            List<IMessage> messages;
            if (amount + skip <= Data.MESSAGE_RETRIEVAL_MAX)
            {
                messages = await Context.Channel.GetMessagesAsync(amount + skip).Flatten().ToList();
                Console.WriteLine(messages.Count());
            }
            else
            {
                messages = await Context.Channel.GetMessagesAsync(Data.MESSAGE_RETRIEVAL_MAX).Flatten().ToList();
                for (int i = 1; i < (amount + skip) / Data.MESSAGE_RETRIEVAL_MAX; i++)
                {
                    messages.AddRange(Context.Channel.GetCachedMessages(Data.MESSAGE_RETRIEVAL_MAX).ToList());
                }
                messages.AddRange(Context.Channel.GetCachedMessages((amount + skip) % Data.MESSAGE_RETRIEVAL_MAX).ToList());
            }
            messages.RemoveRange(0, skip);
            string skipText = "";
            if (skip > 0)
            {
                skipText = "(skipping " + skip + ") ";
            }
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            IUserMessage m = await ReplyAsync(
                embed: new EmbedBuilder()
                .WithColor(Data.COLOR_SUCCESS)
                .WithTitle("Prune successful")
                .WithDescription($"The last {amount} " + skipText + "messages sent in this channel have been deleted.")
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("PruneFrom", RunMode = RunMode.Async)]
        [Alias("PruneF")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        [Summary("Deletes the last x messages sent in the text channel.")]
        public async Task PruneFromMessageAsync(
            [Summary("The id of the message you want to delete from.")]
            [Example("515268392219967498")]
            ulong messageID,
            [Summary("How many messages you want to skip before starting to delete.")]
            [Example("2")]
            int skip = 0)
        {
            await Context.Message.DeleteAsync();
            List<IMessage> messages = new List<IMessage>();
            messages.Add(await Context.Channel.GetMessageAsync(messageID));
            while (messages.LastOrDefault().Id != Context.Channel.GetMessagesAsync(1).Flatten().ToList().GetAwaiter().GetResult()[0].Id)
            {
                List<IMessage> tempList = await Context.Channel.GetMessagesAsync(messages.LastOrDefault().Id, Direction.After).Flatten().ToList();
                tempList.Reverse();
                messages.AddRange(tempList);
            }
            int amount = messages.Count();
            messages.Reverse();
            messages.RemoveRange(0, skip);
            string skipText = "";
            if (skip > 0)
            {
                skipText = "(skipping " + skip + ") ";
            }
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            IUserMessage m = await ReplyAsync(
                embed: new EmbedBuilder()
                .WithColor(Data.COLOR_SUCCESS)
                .WithTitle("Prune successful")
                .WithDescription($"The last {amount} " + skipText + "messages sent in this channel have been deleted.")
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("Move", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        [Summary("Moves a specified amount of messages, after another specified amount of messages, sent in the text channel to another channel provided by id or tag.")]
        public async Task MoveMessagesAsync(
            [Summary("How many messages you want to move.")]
            [Example("10")]
            int amount,
            [Summary("The channel (mention or id) you wish to move the messages to")]
            [Example("#general")]
            [Example("238253605784649728")]
            string channel,
            [Summary("How many messages you want to skip before starting to move.")]
            [Example("2")]
            int skip = 0)
        {
            await Context.Message.DeleteAsync();
            IUserMessage m;
            channel = Regex.Replace(channel, "[^0-9]", "");
            if (ulong.TryParse(channel, out ulong channelId))
            {
                SocketTextChannel textChannel = (SocketTextChannel)Context.Guild.GetChannel(channelId);

                List<IMessage> messages;
                if (amount + skip <= Data.MESSAGE_RETRIEVAL_MAX)
                {
                    messages = await Context.Channel.GetMessagesAsync(amount + skip).Flatten().ToList();
                    Console.WriteLine(messages.Count());
                }
                else
                {
                    messages = await Context.Channel.GetMessagesAsync(Data.MESSAGE_RETRIEVAL_MAX).Flatten().ToList();
                    for (int i = 1; i < (amount + skip) / Data.MESSAGE_RETRIEVAL_MAX; i++)
                    {
                        messages.AddRange(Context.Channel.GetCachedMessages(Data.MESSAGE_RETRIEVAL_MAX).ToList());
                    }
                    messages.AddRange(Context.Channel.GetCachedMessages((amount + skip) % Data.MESSAGE_RETRIEVAL_MAX).ToList());
                }
                messages.RemoveRange(0, skip);

                string warningText = "";
                int x = 0;
                messages.ForEach(msg => x += msg.Content.Length);
                if (x > Data.CHAR_LIMIT * 5)
                {
                    warningText = ", this may take a while.";
                }
                string message = "Conversation moved from " + ((SocketTextChannel)Context.Channel).Mention + " by " + Context.User.Mention + ".\n(" + messages.Count() + " messages moved" + warningText + ")";
                for (int i = messages.Count() - 1; i >= 0; i--)
                {
                    string text = messages[i].Author.Mention + ": " + messages[i].Content;
                    if ((message + "\n" + text).Length < Data.CHAR_LIMIT)
                    {
                        message += "\n" + text;
                        if (messages[i].Attachments.Count() > 0)
                        {
                            Console.WriteLine(messages[i].Attachments.Count());
                            foreach (IAttachment a in messages[i].Attachments)
                            {
                                try
                                {
                                    WebRequest req = WebRequest.Create(a.Url);
                                    WebResponse response = req.GetResponse();
                                    Stream stream = response.GetResponseStream();
                                    await textChannel.SendFileAsync(stream, "attacment" + Path.GetExtension(a.Url), message);
                                }
                                catch
                                {
                                    Console.WriteLine("Shit went sideways in the move command");
                                }
                            }
                            message = "";
                        }
                    }
                    else
                    {
                        await textChannel.SendMessageAsync(message);
                        message = text;
                    }
                }
                await textChannel.SendMessageAsync(message);

                string skipText = "";
                if (skip > 0)
                {
                    skipText = "(skipping " + skip + ") ";
                }
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                m = await ReplyAsync(
                    embed: new EmbedBuilder()
                    .WithColor(Data.COLOR_SUCCESS)
                    .WithTitle("Move successful")
                    .WithDescription($"The last {amount} " + skipText + "messages sent in this channel have been moved to " + textChannel.Mention + ".")
                    .WithAutoDeletionFooter()
                    .Build());
            }
            else
            {
                m = await ReplyAsync(
                    embed: new EmbedBuilder()
                    .WithColor(Data.COLOR_ERROR)
                    .WithTitle("Move unsuccessful")
                    .WithDescription($"Could not find channel with id: `" + channel + "´. Please make sure you post a valid channel id.")
                    .WithAutoDeletionFooter()
                    .Build());
            }
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("MoveFrom", RunMode = RunMode.Async)]
        [Alias("MoveF")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        [Summary("Moves a specified amount of messages, after another specified amount of messages, sent in the text channel to another channel provided by id or tag.")]
        public async Task MoveFromMessageAsync(
            [Summary("The id of the message you want to move from.")]
            [Example("515268392219967498")]
            ulong messageID,
            [Summary("The channel (mention or id) you wish to move the messages to")]
            [Example("#general")]
            [Example("238253605784649728")]
            string channel,
            [Summary("How many messages you want to skip before starting to move.")]
            [Example("2")]
            int skip = 0)
        {
            await Context.Message.DeleteAsync();
            IUserMessage m;
            channel = Regex.Replace(channel, "[^0-9]", "");
            if (ulong.TryParse(channel, out ulong channelId))
            {
                SocketTextChannel textChannel = (SocketTextChannel)Context.Guild.GetChannel(channelId);

                List<IMessage> messages = new List<IMessage>();
                messages.Add(await Context.Channel.GetMessageAsync(messageID));
                while (messages.LastOrDefault().Id != Context.Channel.GetMessagesAsync(1).Flatten().ToList().GetAwaiter().GetResult()[0].Id)
                {
                    List<IMessage> tempList = await Context.Channel.GetMessagesAsync(messages.LastOrDefault().Id, Direction.After).Flatten().ToList();
                    tempList.Reverse();
                    messages.AddRange(tempList);
                }
                int amount = messages.Count();
                messages.Reverse();
                messages.RemoveRange(0, skip);

                string warningText = "";
                int x = 0;
                messages.ForEach(msg => x += msg.Content.Length);
                if (x > Data.CHAR_LIMIT * 5)
                {
                    warningText = ", this may take a while.";
                }
                string message = "Conversation moved from " + ((SocketTextChannel)Context.Channel).Mention + " by " + Context.User.Mention + ".\n(" + messages.Count() + " messages moved" + warningText + ")";
                for (int i = messages.Count() - 1; i >= 0; i--)
                {
                    string text = messages[i].Author.Mention + ": " + messages[i].Content;
                    if ((message + "\n" + text).Length < Data.CHAR_LIMIT)
                    {
                        message += "\n" + text;
                        if (messages[i].Attachments.Count() > 0)
                        {
                            Console.WriteLine(messages[i].Attachments.Count());
                            foreach (IAttachment a in messages[i].Attachments)
                            {
                                try
                                {
                                    WebRequest req = WebRequest.Create(a.Url);
                                    WebResponse response = req.GetResponse();
                                    Stream stream = response.GetResponseStream();
                                    await textChannel.SendFileAsync(stream, "attacment" + Path.GetExtension(a.Url), message);
                                }
                                catch
                                {
                                    Console.WriteLine("Shit went sideways in the move command");
                                }
                            }
                            message = "";
                        }
                    }
                    else
                    {
                        await textChannel.SendMessageAsync(message);
                        message = text;
                    }
                }
                await textChannel.SendMessageAsync(message);

                string skipText = "";
                if (skip > 0)
                {
                    skipText = "(skipping " + skip + ") ";
                }
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                m = await ReplyAsync(
                    embed: new EmbedBuilder()
                    .WithColor(Data.COLOR_SUCCESS)
                    .WithTitle("Move successful").WithDescription($"The last {amount} " + skipText + "messages sent in this channel have been moved to " + textChannel.Mention + ".")
                    .WithAutoDeletionFooter()
                    .Build());
            }
            else
            {
                m = await ReplyAsync(
                    embed: new EmbedBuilder()
                    .WithColor(Data.COLOR_ERROR)
                    .WithTitle("Move unsuccessful").WithDescription($"Could not find channel with id: `" + channel + "´. Please make sure you post a valid channel id.")
                    .WithAutoDeletionFooter()
                    .Build());
            }
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }
    }
}
