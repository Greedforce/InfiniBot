using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace InfiniBot
{
    public enum RoleType
    {
        Admin = 0,
        Color,
        Game,
        Other,
        Last
    }



    class Data
    {
        public const string
            BOT_PREFIX = "!",
            FILE_PATH = "BotFiles\\",
            TOKEN_PATH = "token.json",
            ROLE_PATH = "role.json",
            REMINDER_PATH = "reminder.json",
            REQUESTS_AWAITING_APPROVAL_FILE_NAME = "RequestsAwaitingApproval.txt",
            URL_IMAGE_INFINITY_GAMING = "https://i.imgur.com/hQR0KSE.png",
            URL_ERROR_ICON = "https://i.imgur.com/HSrsLjE.png",
            EMBED_FOOTER_DELETE = "(This message will delete itself in 5 seconds)";

        public const ulong
            GUILD_ID_INFINITY_GAMING = 238251468384108545,
            CHANNEL_ID_GENERAL = 238288636527902730,
            CHANNEL_ID_REQUEST_VOTING = 486212280653185025,
            CHANNEL_ID_ADMIN = 248126773869543424,
            MESSAGE_ID_ADMIN_SERVER_REQUEST_INFO = 491946885624496129;

        public const int
            CHAR_LIMIT = 2000,
            MESSAGE_RETRIEVAL_MAX = 100,
            MESSAGE_DELETE_DELAY = 5;

        public static readonly Color
            COLOR_BOT = new Color(32, 102, 148),
            COLOR_ERROR = new Color(255, 0, 0),
            COLOR_SUCCESS = new Color(0, 255, 0);
        

        
        public static bool HasAdministrativePermission(SocketRole socketRole)
        {
            return socketRole.Permissions.Administrator ||
                socketRole.Permissions.BanMembers ||
                socketRole.Permissions.DeafenMembers ||
                socketRole.Permissions.KickMembers ||
                socketRole.Permissions.ManageChannels ||
                socketRole.Permissions.ManageEmojis ||
                socketRole.Permissions.ManageGuild ||
                socketRole.Permissions.ManageMessages ||
                socketRole.Permissions.ManageNicknames ||
                socketRole.Permissions.ManageRoles ||
                socketRole.Permissions.ManageWebhooks ||
                socketRole.Permissions.MentionEveryone ||
                socketRole.Permissions.MoveMembers ||
                socketRole.Permissions.MuteMembers ||
                socketRole.Permissions.PrioritySpeaker;
        }

        public static bool ReceivedAdministrativePermission(SocketRole prevSocketRole, SocketRole socketRole)
        {
            return (!prevSocketRole.Permissions.Administrator && socketRole.Permissions.Administrator) ||
                    (!prevSocketRole.Permissions.BanMembers && socketRole.Permissions.BanMembers) ||
                    (!prevSocketRole.Permissions.DeafenMembers && socketRole.Permissions.DeafenMembers) ||
                    (!prevSocketRole.Permissions.KickMembers && socketRole.Permissions.KickMembers) ||
                    (!prevSocketRole.Permissions.ManageChannels && socketRole.Permissions.ManageChannels) ||
                    (!prevSocketRole.Permissions.ManageEmojis && socketRole.Permissions.ManageEmojis) ||
                    (!prevSocketRole.Permissions.ManageGuild && socketRole.Permissions.ManageGuild) ||
                    (!prevSocketRole.Permissions.ManageMessages && socketRole.Permissions.ManageMessages) ||
                    (!prevSocketRole.Permissions.ManageNicknames && socketRole.Permissions.ManageNicknames) ||
                    (!prevSocketRole.Permissions.ManageRoles && socketRole.Permissions.ManageRoles) ||
                    (!prevSocketRole.Permissions.ManageWebhooks && socketRole.Permissions.ManageWebhooks) ||
                    (!prevSocketRole.Permissions.MentionEveryone && socketRole.Permissions.MentionEveryone) ||
                    (!prevSocketRole.Permissions.MoveMembers && socketRole.Permissions.MoveMembers) ||
                    (!prevSocketRole.Permissions.MuteMembers && socketRole.Permissions.MuteMembers) ||
                    (!prevSocketRole.Permissions.PrioritySpeaker && socketRole.Permissions.PrioritySpeaker);
        }

        public static List<T> GetContainers<T>(string path)
        {
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
                fs.Dispose();
            }
            string inputJson = File.ReadAllText(path);

            List<T> containers = JsonConvert.DeserializeObject<List<T>>(inputJson);
            if (containers != null)
            {
                return containers;
            }
            else
            {
                return new List<T>();
            }
        }

        public static void SaveContainers<T>(List<T> containers, string path)
        {
            string outputJson = JsonConvert.SerializeObject(containers, Formatting.Indented);
            File.WriteAllText(path, outputJson);
        }

        public static void AddContainer<T>(T container, string path)
        {
            List<T> containers = GetContainers<T>(path);

            containers.Add(container);

            string outputJson = JsonConvert.SerializeObject(containers, Formatting.Indented);
            File.WriteAllText(path, outputJson);
        }

        public static void RemoveContainer<T>(T container, string path)
        {
            List<T> containers = GetContainers<T>(path);

            var c = containers.FirstOrDefault(x => x.Equals(container));
            containers.Remove(c);

            string outputJson = JsonConvert.SerializeObject(containers, Formatting.Indented);
            File.WriteAllText(path, outputJson);
        }

        public static EmbedBuilder AddRoleFields(EmbedBuilder builder, bool includeUnjoinable = false, RoleType roleType = RoleType.Last)
        {
            // Get the RoleContainers.
            List<RoleContainer> roleContainers = GetContainers<RoleContainer>(ROLE_PATH);

            // Roles are put into diffent fields depending on their roleType and neatly stacked for the return message.
            // If a RoleType has been specified it skips the other types. 
            int start = 0,
                end = (int)RoleType.Last;
            if (roleType != RoleType.Last)
            {
                start = (int)roleType;
                end = (int)roleType + 1;
            }
            for (int i = start; i < end; i++)
            {
                string roles = "";
                List<RoleContainer> rcs = roleContainers.Where(rc => (int)rc.roleType == i).ToList();
                rcs.Sort((a, b) => a.name.CompareTo(b.name));
                if (rcs.Count() > 0)
                {
                    for (int j = 0; j < rcs.Count(); j++)
                    {
                        if ((rcs[j].joinable || includeUnjoinable) && rcs[j].name != "@everyone")
                        {
                            if (j > 0)
                            {
                                roles += "\n";
                            }
                            roles += rcs[j].name;
                            if (includeUnjoinable)
                            {
                                roles += " (";
                                if (!rcs[j].joinable)
                                {
                                    roles += "un";
                                }
                                roles += "joinable)";
                            }
                        }
                    }
                }
                else
                {
                    roles += "None";
                }
                builder.AddField(((RoleType)i).ToString(), roles);
            }
            return builder;
        }

        public static Embed GetJoinEmbed(SocketGuild Guild)
        {
            string description = "**Welcome to " + Guild.Name + "!**";
            description += "\n\n**Roles and Games**";
            description += "\nWhen you first join the server you won't have a lot of text and voice channels that are visible to you. This is because you do not have the roles necessary to see them.";
            description += "\nIn order to access the channels of the games you play or would like to discuss, please use the `!Join <role>` command, in any text channel on the server, to have the appropriate role added to you.";
            description += " Similarly, you can use the `!Leave <role>` command to have roles removed from you.";
            description += "\nThere are certain roles that exist only to change the color of your name and have no further purpose beyond this. These are joined and left just like any other role.";
            description += "\n\nTo see a list of the different games/roles you can join to see the corresponding chat rooms, use the `!Games` or `!Roles` command and I will PM you a list of the available roles.";
            description += "\nNote: Don't worry about cluttering text channels. The command automatically removes both the prompt and feedback message after 5 seconds.";
            description += "\n\n**Rules**";
            description += "\nA full list can be found in the `Rules` text channel, but common sense goes a long way.";
            description += "\nWe would however ask that you keep to the appropriate channels (for example: don't post things related to Overwatch in the Dota 2 channel).";
            description += "\n\n\n**Enjoy your stay!**\n/The " + Guild.Name + " crew.";

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithDescription(description)
                .WithColor(COLOR_BOT)
                .WithThumbnailUrl(URL_IMAGE_INFINITY_GAMING)
                .WithFooter("p.s. If you have any further questions, feel free to ask an admin. (Or use the `!Help` command.)");

            return builder.Build();
        }

        public static EmbedBuilder GetFeedbackEmbedBuilder()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithColor(COLOR_BOT)
                .WithFooter(EMBED_FOOTER_DELETE);

            return builder;
        }

        public static string RGBToHexCode(int R, int G, int B)
        {
            return "#" + R.ToString("X") + G.ToString("X") + B.ToString("X");
        }

        public static Tuple<int,int,int> HexCodeToRGB(string hexCode)
        {
            return new Tuple<int, int, int>(
                int.Parse(hexCode.Remove(0, 1).Remove(3, 4), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hexCode.Remove(0, 3).Remove(4, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hexCode.Remove(0, 5), System.Globalization.NumberStyles.HexNumber));
        }
    }
}
