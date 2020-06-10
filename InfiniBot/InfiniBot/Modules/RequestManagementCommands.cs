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
            List<Attachment> attachments = Context.Message.Attachments.ToList();
            foreach (var a in attachments)
            {
                request += ", " + a.Url;
            }
            if (!IsDuplicate(request))
            {
                AddRequest(request);

                UpdateServerRequestInfo();

                // Inform admins
                await Context.Guild.GetTextChannel(Data.CHANNEL_ID_ADMIN).SendMessageAsync(embed: Data.GetFeedbackEmbedBuilder()
                    .WithTitle("New Request Added!")
                    .WithDescription(Context.Guild.GetRole(Data.ROLE_ID_ADMIN).Mention + ", a new request was added by a user and awaits approval")
                    .Build());

                EmbedBuilder builder = new EmbedBuilder();
                builder.WithColor(Data.COLOR_SUCCESS)
                    .WithTitle("Request Added!")
                    .WithDescription("Your request has been added to the list for approval. Once it has been approved by an admin it will be available for voting in " + ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).Mention + ".")
                    .WithFooter("(This message will delete itself in " + Data.MESSAGE_DELETE_DELAY + " seconds)");
                IMessage m = await ReplyAsync(embed: builder.Build());
                await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
                await Context.Message.DeleteAsync();
                await m.DeleteAsync();
            }
            else
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithColor(Data.COLOR_ERROR)
                    .WithTitle("FAILED - Request Added!")
                    .WithDescription("Your request hasn't been added to the list for approval, due to an identical request already existing. If you wish to see what requests are currently awaiting approval, use the \"request display\" command.")
                    .WithFooter("(This message will delete itself in " + Data.MESSAGE_DELETE_DELAY + " seconds)");
                IMessage m = await ReplyAsync(embed: builder.Build());
                await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
                await Context.Message.DeleteAsync();
                await m.DeleteAsync();
            }
        }

        [Command("Display")]
        [Summary("PMs the user a list of all the server requests that are currently pending approval.")]
        public async Task DisplayRequestAsync()
        {
            List<string> requests = GetRequests();
            string message = "There are currently `" + requests.Count() + "` requests awaiting approval:", text;
            for (int i = 0; i < requests.Count(); i++)
            {
                text = "#" + (i + 1) + ": " + requests[i];
                if ((message + "\n" + text).Length < Data.CHAR_LIMIT)
                {
                    message += "\n" + text;
                }
                else
                {
                    await Context.User.SendMessageAsync(message);
                    message = text;
                }
            }
            text = "(Already approved messages can be found and voted on in " + ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).Mention + ")";
            if ((message + "\n" + text).Length < Data.CHAR_LIMIT)
            {
                message += "\n" + text;
                await Context.User.SendMessageAsync(message);
            }
            else
            {
                await Context.User.SendMessageAsync(message);
                await Context.User.SendMessageAsync(text);
            }
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Data.COLOR_SUCCESS)
                .WithTitle("Request Display!")
                .WithDescription("I have PMed you the list of requests.")
                .WithFooter("(This message will delete itself in " + Data.MESSAGE_DELETE_DELAY + " seconds)");
            IMessage m = await ReplyAsync(embed: builder.Build());
            Data.tempMessages.Add(new TempMessage(m, Data.MESSAGE_DELETE_DELAY));
            Data.tempMessages.Add(new TempMessage(Context.Message, Data.MESSAGE_DELETE_DELAY));
        }

        [Command("Deny")]
        [Summary("Removes one or more server requests pending approval from the list.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DenyRequestAsync([Summary("The Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.")]string inputIndex)
        {
            EmbedBuilder builder = new EmbedBuilder();
            if (inputIndex.ToLower() == "all")
            {
                while (GetRequests().Count() > 0)
                {
                    RemoveRequest(0);
                }

                builder.WithColor(Data.COLOR_SUCCESS)
                    .WithTitle("All Requests Denied!")
                    .WithDescription("All requests have been denied and removed from the list.");
            }
            else if (int.TryParse(inputIndex, out int index))
            {
                if (index > 0 && index <= GetRequests().Count())
                {
                    index--; //So the index matches the arays and lists, starting at 0.
                    string output = RemoveRequest(index);

                    builder.WithColor(Data.COLOR_SUCCESS)
                        .WithTitle("Request Denied!")
                        .WithDescription(output);
                }
                else
                {
                    builder.WithColor(Data.COLOR_ERROR)
                        .WithTitle("FAILED - Request Denied!")
                        .WithDescription("`" + inputIndex + "` is invalid. Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.");
                }
            }
            else
            {
                builder.WithColor(Data.COLOR_ERROR)
                    .WithTitle("FAILED - Request Denied!")
                    .WithDescription("`" + inputIndex + "` is invalid. Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.");
            }

            UpdateServerRequestInfo();

            builder.WithFooter("(This message will delete itself in " + Data.MESSAGE_DELETE_DELAY + " seconds)");
            IMessage m = await ReplyAsync(embed: builder.Build());
            Data.tempMessages.Add(new TempMessage(m, Data.MESSAGE_DELETE_DELAY));
            Data.tempMessages.Add(new TempMessage(Context.Message, Data.MESSAGE_DELETE_DELAY));
        }

        [Command("Approve")]
        [Summary("Approves one or more server requests pending approval and makes them available for voting.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ApproveRequestAsync([Summary("The Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.")]string inputIndex)
        {
            EmbedBuilder builder = new EmbedBuilder();
            if (inputIndex.ToLower() == "all")
            {
                while (GetRequests().Count() > 0)
                {
                    ApproveRequest(0);
                }

                builder.WithColor(Data.COLOR_SUCCESS)
                    .WithTitle("All Requests Approved!")
                    .WithDescription("All requests have been approved and posted in " + ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).Mention + " for voting.");
            }
            else if (int.TryParse(inputIndex, out int index))
            {
                if (index > 0 && index <= GetRequests().Count())
                {
                    index--; //So the index matches the arays and lists, starting at 0.
                    string output = ApproveRequest(index);

                    builder.WithColor(Data.COLOR_SUCCESS)
                        .WithTitle("Request Approved!")
                        .WithDescription(output);
                }
                else
                {
                    builder.WithColor(Data.COLOR_ERROR)
                        .WithTitle("FAILED - Request Approved!")
                        .WithDescription("`" + inputIndex + "` is invalid. Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.");
                }
            }
            else
            {
                builder.WithColor(Data.COLOR_ERROR)
                    .WithTitle("FAILED - Request Approved!")
                    .WithDescription("`" + inputIndex + "` is invalid. Input must be \"all\" or a digit, greater than 0 and lower or equal to the number of requests pending approval.");
            }

            UpdateServerRequestInfo();

            builder.WithFooter("(This message will delete itself in " + Data.MESSAGE_DELETE_DELAY + " seconds)");
            IMessage m = await ReplyAsync(embed: builder.Build());
            Data.tempMessages.Add(new TempMessage(m, Data.MESSAGE_DELETE_DELAY));
            Data.tempMessages.Add(new TempMessage(Context.Message, Data.MESSAGE_DELETE_DELAY));
        }

        public List<string> GetRequests()
        {
            List<string> requests = new List<string>();

            StreamReader objReader = new StreamReader(Data.FILE_PATH + Data.REQUESTS_AWAITING_APPROVAL_FILE_NAME);
            string sLine = "";

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    requests.Add(sLine);
                }
            }
            objReader.Close();

            return requests;
        }

        public string GetRequest(int i)
        {
            return GetRequests()[i];
        }

        public void AddRequest(string request)
        {
            List<string> existingRequests = GetRequests();

            StreamWriter objwriter = new StreamWriter(Data.FILE_PATH + Data.REQUESTS_AWAITING_APPROVAL_FILE_NAME);

            foreach (string r in existingRequests)
            {
                objwriter.WriteLine(r);
            }
            objwriter.WriteLine(request);

            objwriter.Close();
        }

        public string RemoveRequest(int index)
        {
            List<string> existingRequests = GetRequests();
            string request = existingRequests[index];

            StreamWriter objwriter = new StreamWriter(Data.FILE_PATH + Data.REQUESTS_AWAITING_APPROVAL_FILE_NAME);

            for (int i = 0; i < existingRequests.Count(); i++)
            {
                if (i != index)
                {
                    objwriter.WriteLine(existingRequests[i]);
                }
            }

            objwriter.Close();

            index++; //So the number matches the list, starting at 1 again.
            return "The request at the `" + index + GetIndexEnding(index) + "` position have been denied and removed from the list.\nDenied request:\n`" + request + "`";
        }

        public string ApproveRequest(int index)
        {
            string request = GetRequest(index);
            var m = ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).SendMessageAsync(request).GetAwaiter().GetResult();
            m.AddReactionAsync(new Emoji("👍")).GetAwaiter().GetResult();
            m.AddReactionAsync(new Emoji("👎")).GetAwaiter().GetResult();
            RemoveRequest(index);

            index++; //So the number matches the list, starting at 1 again.
            return "The request at the `" + index + GetIndexEnding(index) + "` position have been approved and posted in " + ((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_REQUEST_VOTING)).Mention + " for voting.\nApproved request:\n`" + request + "`";
        }

        public void UpdateServerRequestInfo()
        {
            RestUserMessage message = (RestUserMessage)((SocketTextChannel)Context.Guild.GetChannel(Data.CHANNEL_ID_ADMIN)).GetMessageAsync(Data.MESSAGE_ID_ADMIN_SERVER_REQUEST_INFO).GetAwaiter().GetResult();

            message.ModifyAsync(msg =>
            {
                msg.Content = "";
                msg.Embed = new EmbedBuilder()
                .WithColor(Data.COLOR_BOT)
                .WithTitle("Server Request Info")
                .WithDescription("Current number of requests pending approval: `" + GetRequests().Count() + "`")
                .WithFooter("(Do not remove this message! It is edited by the bot whenever the above information changes to keep it updated. Removing this message will cause the bot to crash!)")
                .Build();
            }).GetAwaiter().GetResult();
        }

        public string GetIndexEnding(int index)
        {
            string position;
            if (index % 10 == 1)
            {
                position = "st";
            }
            else if (index % 10 == 2)
            {
                position = "nd";
            }
            else if (index % 10 == 3)
            {
                position = "rd";
            }
            else
            {
                position = "th";
            }
            return position;
        }

        public bool IsDuplicate(string request)
        {
            List<string> requests = GetRequests();

            foreach (string r in requests)
            {
                if (r == request)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
