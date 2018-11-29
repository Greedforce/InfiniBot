using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace InfiniBot.Modules
{
    public class RoleManagement : ModuleBase<SocketCommandContext>
    {
        [Command("Roles", RunMode = RunMode.Async)]
        [Summary("PMs the user a list of all available roles")]
        public async Task DisplayRolesAsync(
            [Summary("The type of roles you want a list of. If left empty, this will default to show all roles")]
            [Example("Admin")]
            [Example("Color")]
            [Example("Game")]
            [Example("Other")]
            RoleType type = RoleType.Last)
        {
            // Declare EmbedBuilder base.
            EmbedBuilder builder = new EmbedBuilder()
                .WithColor(Data.COLOR_BOT)
                .WithThumbnailUrl(Data.URL_IMAGE_INFINITY_GAMING)
                .WithTitle("Roles")
                .WithDescription($"Here is a list of every role on `{Context.Guild.Name}`, sorted into types:");

            // Get the role list fields to the EmbedBuilder.
            builder = Data.AddRoleFields(builder, ((SocketGuildUser)Context.User).GuildPermissions.Administrator, type);

            // Send role list to user and return feedback message.
            await Context.User.SendMessageAsync(embed: builder.Build());

            string description = "I have PMed you a list of the ";
            if(type == RoleType.Last)
            {
                description += "server";
            }
            else
            {
                description += $"`{type.ToString()}`";
            }
            description += " roles.";
            IMessage m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithTitle("Role list sent").WithDescription(description).Build());

            // Delete prompt and feedback messages.
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await Context.Message.DeleteAsync();
            await m.DeleteAsync();
        }

        [Command("Preview", RunMode = RunMode.Async)]
        [Summary("PMs a preview of the specified role to the user.")]
        public async Task PreviewRoleAsync(
            [Summary("The name of the role(s) the user wishes to preview. Role names are seperated by ','s.")]
            [Example("Overwatch")]
            [Example("Overwatch, Dota 2, Payday 2")]
            [Remainder]
            string roles)
        {
            List<IMessage> messages = new List<IMessage>();

            List<string> roleNames = roles.Split(',').ToList();
            for (int i = 0; i < roleNames.Count; i++)
            {
                roleNames[i] = roleNames[i].Trim();
            }

            foreach (string rn in roleNames)
            {
                // Autocorrecting goes here.

                // Get the roleContainer corresponding to the role the user want to preview.
                SocketRole socketRole = Context.Guild.Roles.FirstOrDefault(sr => sr.Name.ToLower() == rn.ToLower());
                if (socketRole != null)
                {
                    // Construct preview message.
                    string toReturn = $"\n**Name**: `{socketRole.Name}`";
                    toReturn += $"\n**Color Hex Code**: {Data.RGBToHexCode(socketRole.Color.R, socketRole.Color.G, socketRole.Color.B)}";
                    toReturn += $"\n**Color RGB**: {socketRole.Color.R}, {socketRole.Color.G}, {socketRole.Color.B}";

                    // Get channels associated with the role and sort them into text and voice channels.
                    List<SocketGuildChannel> associatedChannels = Context.Guild.Channels.Where(c => c.GetPermissionOverwrite(socketRole).HasValue).ToList();
                    List<SocketTextChannel> textChannels = new List<SocketTextChannel>();
                    List<SocketVoiceChannel> voiceChannels = new List<SocketVoiceChannel>();
                    if (associatedChannels != null)
                    {
                        foreach (SocketGuildChannel sgc in associatedChannels)
                        {
                            if (sgc is SocketTextChannel)
                            {
                                textChannels.Add(sgc as SocketTextChannel);
                            }
                            if (sgc is SocketVoiceChannel)
                            {
                                voiceChannels.Add(sgc as SocketVoiceChannel);
                            }
                        }
                    }

                    // Add text and voice channels to the preview message.
                    toReturn += "\n\n**Associated Text Channels:**";
                    if (textChannels.Count() > 0)
                    {
                        foreach (SocketTextChannel tc in textChannels)
                        {
                            toReturn += $"\n{tc.Name}";
                        }
                    }
                    else
                    {
                        toReturn += "\nNone";
                    }
                    toReturn += "\n\n**Associated Voice Channels:**";
                    if (voiceChannels.Count() > 0)
                    {
                        foreach (SocketVoiceChannel vc in voiceChannels)
                        {
                            toReturn += $"\n{vc.Name}";
                        }
                    }
                    else
                    {
                        toReturn += "\nNone";
                    }

                    // PM preview to user.
                    await Context.User.SendMessageAsync(embed: new EmbedBuilder().WithColor(socketRole.Color).WithTitle("Role Preview").WithDescription(toReturn).Build());
                }
                else
                {
                    messages.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("ERROR: Role not found").WithDescription($"Could not find a role matching the name `{rn}`.\nFor a full list of roles, use the `roles` command.").Build()));
                }
            }

            // Return feedback message.
            messages.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithTitle("Preview(s) sent").WithDescription("I've PMed you the role preview(s).").Build()));

            // Delete prompt and feedback messages.
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await Context.Message.DeleteAsync();
            foreach (var msg in messages)
            {
                await msg.DeleteAsync();
            }
        }

        [Command("Join", RunMode = RunMode.Async)]
        [Alias("J")]
        [Summary("Adds the specified role to the user.")]
        public async Task JoinRoleAsync(
            [Summary("The name of the role(s) the user wishes to join. Role names are seperated by ','s.")]
            [Example("Overwatch")]
            [Example("Overwatch, Dota 2, Payday 2")]
            [Remainder]
            string roles)
        {
            List<IMessage> messages = new List<IMessage>();
            
            List<string> roleNames = roles.Split(',').ToList();
            for (int i = 0; i < roleNames.Count; i++)
            {
                roleNames[i] = roleNames[i].Trim();
            }

            foreach (string rn in roleNames)
            {
                // Autocorrecting goes here.

                // Get the roleContainer corresponding to the role the user want to join.
                List<RoleContainer> roleContainers = Data.GetContainers<RoleContainer>(Data.ROLE_PATH);
                RoleContainer roleContainer = roleContainers.FirstOrDefault(rc => rc.name.ToLower() == rn.ToLower());
                
                if (roleContainer != null)
                {
                    if (((SocketGuildUser)Context.User).Roles.FirstOrDefault(r => r.Name.ToLower() == roleContainer.name.ToLower()) != null)
                    {
                        messages.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("User already in role").WithDescription($"You already have the `{roleContainer.name}` role. If you wanna leave the role, use the `leave` command.").Build()));
                    }
                    else
                    {
                        if (roleContainer.joinable)
                        {
                            // If the role is a color, remove all other color roles from the user.
                            if(roleContainer.roleType == RoleType.Color)
                            {
                                List<IRole> colorRoles = new List<IRole>();
                                foreach (SocketRole r in ((SocketGuildUser)Context.User).Roles)
                                {
                                    RoleContainer colorRoleContainer = roleContainers.FirstOrDefault(rc => rc.name.ToLower() == r.Name.ToLower());
                                    if(colorRoleContainer != null)
                                    {
                                        if (colorRoleContainer.roleType == RoleType.Color)
                                        {
                                            colorRoles.Add(r);
                                        }
                                    }
                                }
                                await ((SocketGuildUser)Context.User).RemoveRolesAsync(colorRoles);
                            }

                            // Add role and return feedback message.
                            SocketRole socketRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == roleContainer.name);
                            await ((SocketGuildUser)Context.User).AddRoleAsync(socketRole);
                            messages.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_SUCCESS).WithTitle("Role joined").WithDescription($"You have successfully joined the `{socketRole.Name}` role and now have access to all the text and voice channels associated with it.").Build()));
                        }
                        else
                        {
                            messages.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("Access to role denied").WithDescription($"The `{roleContainer.name}` role is not joinable. It is strictly distributed by admins.").Build()));
                        }
                    }
                }
                else
                {
                    messages.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("Role not found").WithDescription($"Could not find a role matching the name `{rn}`.\nFor a full list of roles, use the `roles` command.").Build()));
                }
            }

            // Delete prompt and feedback messages.
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await Context.Message.DeleteAsync();
            foreach (var msg in messages)
            {
                await msg.DeleteAsync();
            }
        }

        [Command("Leave", RunMode = RunMode.Async)]
        [Alias("L")]
        [Summary("Removes the specified role from the user.")]
        public async Task LeaveRoleAsync(
            [Summary("The name of the role(s) the user wishes to leave. Role names are seperated by ','s.")]
            [Example("Overwatch")]
            [Example("Overwatch, Dota 2, Payday 2")]
            [Remainder]
            string roles)
        {
            List<IMessage> msgs = new List<IMessage>();

            List<string> roleNames = roles.Split(',').ToList();
            for (int i = 0; i < roleNames.Count; i++)
            {
                roleNames[i] = roleNames[i].Trim();
            }

            foreach (string rn in roleNames)
            {
                // Autocorrecting goes here.

                // Get the roleContainer corresponding to the role the user want to leave.
                List<RoleContainer> roleContainers = Data.GetContainers<RoleContainer>(Data.ROLE_PATH);
                RoleContainer roleContainer = roleContainers.FirstOrDefault(rc => rc.name.ToLower() == rn.ToLower());

                if (roleContainer != null)
                {
                    if (((SocketGuildUser)Context.User).Roles.FirstOrDefault(r => r.Name.ToLower() == roleContainer.name.ToLower()) == null)
                    {
                        msgs.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("User not in role").WithDescription($"You do not have the `{roleContainer.name}` role. If you wanna join the role, use the `join` command.").Build()));
                    }
                    else
                    {
                        if (roleContainer.joinable)
                        {
                            // Add role and return feedback message.
                            SocketRole socketRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower() == roleContainer.name.ToLower());
                            await ((SocketGuildUser)Context.User).RemoveRoleAsync(socketRole);
                            msgs.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_SUCCESS).WithTitle("Role Left").WithDescription($"You have successfully left the `{socketRole.Name}` role and no longer have access to all the text and voice channels associated with it.").Build()));
                        }
                        else
                        {
                            msgs.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("Access to role denied").WithDescription($"The `{roleContainer.name}` role is not leaveable. It is strictly distributed by admins.").Build()));
                        }
                    }
                }
                else
                {
                    msgs.Add(await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("Role not found").WithDescription($"Could not find a role matching the name `{rn}`.\nFor a full list of roles, use the `roles` command.").Build()));
                }
            }

            // Delete prompt and feedback messages.
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await Context.Message.DeleteAsync();
            foreach (var msg in msgs)
            {
                await msg.DeleteAsync();
            }
        }

        [Command("Scan", RunMode = RunMode.Async)]
        [Summary("PMs the user a list of all available roles")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task ScanRolesAsync()
        {
            // Get all SocketRoles on the server and give them properties corresponding to their permission level.
            // This is done to avoid administrative roles being joinable directly after the scan and to minimize the extra work needed by admins.
            List<RoleContainer> roleContainers = new List<RoleContainer>();
            foreach (SocketRole sr in Context.Guild.Roles)
            {
                bool joinable = false;
                RoleType roleType = RoleType.Other;
                if (Data.HasAdministrativePermission(sr))
                {
                    joinable = false;
                    roleType = RoleType.Admin;
                }
                else if (sr.Permissions.Connect ||
                    sr.Permissions.SendMessages ||
                    sr.Permissions.SendTTSMessages ||
                    sr.Permissions.Speak ||
                    sr.Permissions.ViewChannel)
                {
                    joinable = true;
                    roleType = RoleType.Game;
                }
                roleContainers.Add(new RoleContainer(sr.Name, joinable, roleType));
            }
            // Sort the roleContainers after roleType and save them to file.
            roleContainers.Sort((a, b) => a.roleType.CompareTo(b.roleType));

            Data.SaveContainers(roleContainers, Data.ROLE_PATH);

            // Return feedback message.
            IMessage m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithTitle("Scan Complete").WithDescription($"All roles on `{Context.Guild.Name}` have now been scanned. To get a list of them, use the `Roles` command. You can also further edit these roles by using the `Edit`, `Add` and `Remove` commands.").Build());

            // Delete prompt and feedback messages.
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await Context.Message.DeleteAsync();
            await m.DeleteAsync();
        }

        [Command("Add", RunMode = RunMode.Async)]
        [Summary("Adds the specified role to the database.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task AddRoleAsync(
            [Summary("The name of the role the user wishes to add (quotation marks are required if the name contains spaces)")]
            [Example("Overwatch")]
            [Example("\"Payday 2\"")]
            string role,
            [Summary("The type the role should be.")]
            [Example("Game")]
            [Example("2")]
            RoleType type,
            [Summary("If the role should be joinable or not.")]
            [Example("true")]
            [Example("false")]
            bool joinable)
        {
            IMessage m;
            // Get SocketRole and check if role exists.
            List<RoleContainer> roleContainers = Data.GetContainers<RoleContainer>(Data.ROLE_PATH);
            SocketRole socketRole = Context.Guild.Roles.FirstOrDefault(sr => sr.Name.ToLower() == role.ToLower());
            if (roleContainers.FirstOrDefault(rc => rc.name.ToLower() == role.ToLower()) == null && socketRole != null)
            {
                // Create RoleContainer and add it to the file.
                RoleContainer roleContainer = new RoleContainer(socketRole.Name, joinable, type);
                Data.AddContainer(roleContainer, Data.ROLE_PATH);

                // Return feedback message.
                m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_SUCCESS).WithTitle("Role added").WithDescription($"The `{roleContainer.name} ({roleContainer.roleType} - {roleContainer.joinable})` role has been successfully added to the database.").Build());
            }
            else
            {
                m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("ERROR: Role not found").WithDescription($"The `{role}` role you specified does not exist.").Build());
            }

            // Delete prompt and feedback messages.
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await Context.Message.DeleteAsync();
            await m.DeleteAsync();
        }

        [Command("Remove", RunMode = RunMode.Async)]
        [Summary("Removes the specified role to the database.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task RemoveRoleAsync(
            [Summary("The name of the role the user wishes to remove")]
            [Example("Overwatch")]
            [Remainder]
            string role)
        {
            IMessage m;
            // Get RoleContainer if it exists.
            List<RoleContainer> roleContainers = Data.GetContainers<RoleContainer>(Data.ROLE_PATH);
            RoleContainer roleContainer = roleContainers.FirstOrDefault(rc => rc.name.ToLower() == role.ToLower());
            if (roleContainer != null)
            {
                // Remove RoleContainer.
                Data.RemoveContainer(roleContainer, Data.ROLE_PATH);

                // Return feedback message.
                m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("Role added").WithDescription($"The `{roleContainer.name}` role has been successfully removed from the database.").Build());
            }
            else
            {
                m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("ERROR: Role not found").WithDescription($"The `{role}` role you specified does not exist.").Build());
            }

            // Delete prompt and feedback messages.
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await Context.Message.DeleteAsync();
            await m.DeleteAsync();
        }

        [Command("Edit", RunMode = RunMode.Async)]
        [Summary("Edits the specified role in the database.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task EditRoleAsync(
            [Summary("The name of the role the user wishes to edit (quotation marks are required if the name contains spaces)")]
            [Example("Overwatch")]
            [Example("\"Payday 2\"")]
            string role,
            [Summary("The type the role should be.")]
            [Example("Game")]
            [Example("2")]
            RoleType type,
            [Summary("If the role should be joinable or not.")]
            [Example("true")]
            [Example("false")]
            bool joinable)
        {
            IMessage m;
            // Get roleContainers and check if role exists.
            List<RoleContainer> roleContainers = Data.GetContainers<RoleContainer>(Data.ROLE_PATH);
            RoleContainer roleContainer = roleContainers.FirstOrDefault(rc => rc.name.ToLower() == role.ToLower());
            if (roleContainer != null && Context.Guild.Roles.FirstOrDefault(sr => sr.Name.ToLower() == role.ToLower()) != null)
            {
                RoleType prevType = roleContainer.roleType;
                bool prevJoinable = roleContainer.joinable;
                roleContainer.roleType = type;
                roleContainer.joinable = joinable;

                Data.SaveContainers(roleContainers, Data.ROLE_PATH);

                // Return feedback message.
                m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_SUCCESS).WithTitle("Role edited").WithDescription($"The `{roleContainer.name}` role has been successfully edited from `{prevType} - {prevJoinable}` to `{roleContainer.roleType} - {roleContainer.joinable}`").Build());
            }
            else
            {
                m = await ReplyAsync(embed: Data.GetFeedbackEmbedBuilder().WithColor(Data.COLOR_ERROR).WithTitle("ERROR: Role not found").WithDescription($"The `{role}` role you specified does not exist.").Build());
            }

            // Delete prompt and feedback messages.
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await Context.Message.DeleteAsync();
            await m.DeleteAsync();
        }
    }
}
