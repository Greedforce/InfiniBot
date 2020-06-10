using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace InfiniBot
{
    public class RequestManagementCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Request")]
        [Summary("Adds a server request. Requests must first be approved by an admin before they can be voted on. Please provide links if possible and relevant.")]
        public async Task AddRequestAsync(
            [Summary("The request you wish to add.")]
            [Example("Channel: cooking channel")]
            [Example("Emoji: Kermit drinking tea")]
            [Remainder]
            string request)
        {
            /* Todo:
             * save attachments to channel, so links work (the original request message getting removed cause the links to fail)
             */

            List<Attachment> attachments = Context.Message.Attachments.ToList();
            foreach (var a in attachments)
            {
                request += ", " + a.Url;
            }
            if (!IsDuplicate(request))
            {
                Data.AddContainer(request, Data.FILE_PATH + Data.REQUEST_FILE);

                UpdateServerRequestInfo();

                await Context.Guild.GetTextChannel(Data.CHANNEL_ID_ADMIN).SendMessageAsync(
                    embed: new EmbedBuilder()
                    .WithTitle("New Request Added!")
                    .WithDescription(Context.Guild.GetRole(Data.ROLE_ID_ADMIN).Mention + ", a new request was added by a user and awaits approval")
                    .WithAutoDeletionFooter()
                    .Build());

                IMessage m = await ReplyAsync(
                    embed: new EmbedBuilder()
                    .WithColor(Data.COLOR_SUCCESS)
                    .WithTitle("Request added successfully")
                    .WithDescription("Your request has been added to the list for approval. Once it has been approved by an admin it will be available for voting in " + ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).Mention + ".")
                    .WithAutoDeletionFooter()
                    .Build());
                await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
                await Context.Message.DeleteAsync();
                await m.DeleteAsync();
            }
            else
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithColor(Data.COLOR_ERROR)
                    .WithTitle("ERROR - Failed to add request")
                    .WithDescription("Your request wasn't been added to the list for approval, due to an identical request already existing. If you wish to see what requests are currently awaiting approval, use the \"request display\" command.")
                    .WithAutoDeletionFooter();
                IMessage m = await ReplyAsync(embed: builder.Build());
                await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
                await Context.Message.DeleteAsync();
                await m.DeleteAsync();
            }
        }

        [Command("DenyRequest")]
        [Alias("DenyR", "Deny")]
        [Summary("Removes one or more server requests pending approval from the list.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DenyRequestAsync([Summary("The Input must be \"all\" or a digit greater than 0 and lower or equal to the number of requests pending approval.")]string inputIndex)
        {
            EmbedBuilder builder = new EmbedBuilder();
            List<string> requests = Data.GetContainers<string>(Data.FILE_PATH + Data.REQUEST_FILE);
            if (requests != null)
            {
                if (inputIndex.ToLower() == "all")
                {
                    while (requests.Count() > 0)
                    {
                        RemoveRequest(0);
                    }

                    builder.WithColor(Data.COLOR_SUCCESS)
                        .WithTitle("All requests denied successfully")
                        .WithDescription("All requests have been denied and removed from the list.");
                }
                else if (int.TryParse(inputIndex, out int index))
                {
                    if (index > 0 && index <= requests.Count())
                    {
                        index--; //So the index matches the arrays and lists, starting at 0.
                        string output = RemoveRequest(index);

                        builder.WithColor(Data.COLOR_SUCCESS)
                            .WithTitle("Request denied successfully")
                            .WithDescription(output);
                    }
                    else
                    {
                        builder.WithColor(Data.COLOR_ERROR)
                            .WithTitle("Failed to deny request")
                            .WithDescription("`" + inputIndex + "` is invalid. Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.");
                    }
                }
                else
                {
                    builder.WithColor(Data.COLOR_ERROR)
                        .WithTitle("Failed to deny request")
                        .WithDescription("`" + inputIndex + "` is invalid. Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.");
                }

            }
            else
            {
                builder.WithColor(Data.COLOR_ERROR)
                    .WithTitle("Failed to deny request")
                    .WithDescription("Unable to find and retrieve request list.");
            }

            UpdateServerRequestInfo();

            IMessage m = await ReplyAsync(
                embed: builder
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("ApproveRequest")]
        [Alias("ApproveR", "Approve")]
        [Summary("Approves one or more server requests pending approval and makes them available for voting.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ApproveRequestAsync([Summary("The Input must be \"all\" or a digit greater than 0 and lower or equal to the number of requests pending approval.")]string inputIndex)
        {
            EmbedBuilder builder = new EmbedBuilder();
            List<string> requests = Data.GetContainers<string>(Data.FILE_PATH + Data.REQUEST_FILE);
            if (requests != null)
            {
                if (inputIndex.ToLower() == "all")
                {
                    while (requests.Count() > 0)
                    {
                        ApproveRequest(0);
                    }

                    builder.WithColor(Data.COLOR_SUCCESS)
                        .WithTitle("All requests approved successfully")
                        .WithDescription("All requests have been approved and posted in " + ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).Mention + " for voting.");
                }
                else if (int.TryParse(inputIndex, out int index))
                {
                    if (index > 0 && index <= requests.Count())
                    {
                        index--; //So the index matches the arrays and lists, starting at 0.
                        string output = ApproveRequest(index);

                        builder.WithColor(Data.COLOR_SUCCESS)
                            .WithTitle("Request approved successfully")
                            .WithDescription(output);
                    }
                    else
                    {
                        builder.WithColor(Data.COLOR_ERROR)
                            .WithTitle("Failed to approve request")
                            .WithDescription("`" + inputIndex + "` is invalid. Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.");
                    }
                }
                else
                {
                    builder.WithColor(Data.COLOR_ERROR)
                        .WithTitle("Failed to approve request")
                        .WithDescription("`" + inputIndex + "` is invalid. Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.");
                }
            }
            else
            {
                builder.WithColor(Data.COLOR_ERROR)
                    .WithTitle("Failed to approve request")
                    .WithDescription("Unable to find and retrieve request list.");
            }

            UpdateServerRequestInfo();

            IMessage m = await ReplyAsync(
                embed: builder
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        [Command("DisplayRequests")]
        [Alias("DisplayRequest", "ShowRequests", "ShowRequest", "Display", "Show")]
        [Summary("PMs the user a list of all the server requests that are currently pending approval.")]
        public async Task DisplayRequestAsync()
        {
            IMessage m;

            string path = Data.FILE_PATH + Data.REQUEST_FILE;

            List<string> requests = Data.GetContainers<string>(path);
            if (requests != null)
            {
                // requests.Count / 
                EmbedBuilder builder = new EmbedBuilder()
                    .WithColor(Data.COLOR_BOT)
                    .WithTitle("Request list (awaiting approval)")
                    .WithDescription("There are currently `" + requests.Count() + "` requests awaiting approval:")
                    .WithFooter("(Already approved messages can be found and voted on in " + ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).Mention + ")");

                for (int i = 0; i < requests.Count(); i++)
                {
                    // add field and send if field limit has been reached
                    builder.AddField("#" + (i + 1) + ":", requests[i]);
                    if (i % Data.EMBED_FIELD_LIMIT == Data.EMBED_FIELD_LIMIT - 1 && i < requests.Count() - 1) // Field 0-24, 25-49, etc
                    {
                        await Context.User.SendMessageAsync(embed: builder.Build());
                        builder.Fields.Clear();
                    }
                }

                m = await ReplyAsync(
                    embed: new EmbedBuilder()
                    .WithColor(Data.COLOR_SUCCESS)
                    .WithTitle("Request Display")
                    .WithDescription("I have PMed you the list of requests.")
                    .WithAutoDeletionFooter()
                    .Build());
            }
            else
            {
                m = await ReplyAsync(
                       embed: new EmbedBuilder()
                       .WithColor(Data.COLOR_ERROR)
                       .WithTitle("Failed to display requests")
                       .WithDescription("Unable to find and retrieve request list.")
                       .WithAutoDeletionFooter()
                       .Build());
            }
            await Task.Delay(Data.MESSAGE_DELETE_DELAY);
            await m.DeleteAsync();
            await Context.Message.DeleteAsync();
        }

        public string RemoveRequest(int index)
        {
            string path = Data.FILE_PATH + Data.REQUEST_FILE;

            List<string> existingRequests = Data.GetContainers<string>(path);
            string request = existingRequests[index];
            if (existingRequests != null)
            {
                Data.RemoveContainer(request, path);
            }

            index++; //So the number matches the list, starting at 1 again.
            return "The request at the `" + index + Data.GetIndexEnding(index) + "` position have been denied and removed from the list.\nDenied request:\n`" + request + "`";
        }

        public string ApproveRequest(int index)
        {
            string path = Data.FILE_PATH + Data.REQUEST_FILE;

            List<string> existingRequests = Data.GetContainers<string>(path);
            string request = existingRequests[index];

            // Put the request up for voting
            RestUserMessage m = ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).SendMessageAsync(request).GetAwaiter().GetResult();
            m.AddReactionAsync(new Emoji("👍")).GetAwaiter().GetResult();
            m.AddReactionAsync(new Emoji("👎")).GetAwaiter().GetResult();
            RemoveRequest(index);

            index++; //So the number matches the list, starting at 1 again.
            return "The request at the `" + index + Data.GetIndexEnding(index) + "` position have been approved and posted in " + ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).Mention + " for voting.\nApproved request:\n`" + request + "`";
        }

        public void UpdateServerRequestInfo()
        {
            EmbedBuilder builder = new EmbedBuilder()
                   .WithColor(Data.COLOR_BOT)
                   .WithTitle("Server Request Info")
                   .WithFooter("(Do not remove this message! It is edited by the bot whenever the above information changes to keep it updated. Removing this message will cause the bot to crash!)");
            List<string> requests = Data.GetContainers<string>(Data.FILE_PATH + Data.REQUEST_FILE);

            if(requests != null)
            {
                builder
                   .WithDescription("Current number of requests pending approval: `" + requests.Count() + "`");
            }
            else
            {
                builder
                   .WithDescription("Current number of requests pending approval: `Error: Unable to find and retrieve request list.`");
            }

            RestUserMessage message = (RestUserMessage)((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_ADMIN)).GetMessageAsync(Data.MESSAGE_ID_ADMIN_SERVER_REQUEST_INFO).GetAwaiter().GetResult();

            message.ModifyAsync(msg =>
            {
                msg.Content = "";
                msg.Embed = builder.Build();
            }).GetAwaiter().GetResult();
        }

        public bool IsDuplicate(string request)
        {
            string path = Data.FILE_PATH + Data.REQUEST_FILE;

            List<string> requests = Data.GetContainers<string>(path);

            if (request != null)
            {
                foreach (string r in requests)
                {
                    if (r == request)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
