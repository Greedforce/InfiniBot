using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Discord;
using Discord.WebSocket;

namespace InfiniBot
{
    class Data
    {
        public const string
            BOT_TOKEN = "MjM4Mjc5ODk5Njg3ODEzMTIw.DZ2xkQ.BD2lwvSIuDVgm1shvqiQcLJG7c4",//InfiniBot Token
            TEST_BOT_TOKEN = "MjE5NDU4NDQyMTc0MTM2MzMw.DXnfxg.zghdN6Te1WHqM_77EHF0914c5as",//DiscordTestBot Token
            BOT_PREFIX = "!",
            FILE_PATH = "BotFiles\\",
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



        public static List<TempMessage> tempMessages = new List<TempMessage>();

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

        public static EmbedBuilder GetFeedbackEmbed()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithColor(COLOR_BOT)
                .WithFooter(EMBED_FOOTER_DELETE);

            return builder;
        }
    }

    public class TempMessage
    {
        public IMessage message { get; set; }
        public Timer timer { get; set; }

        public TempMessage(IMessage message)
        {
            this.message = message;
            timer = new Timer(Data.MESSAGE_DELETE_DELAY * 1000);
            timer.AutoReset = false;
            timer.Elapsed += ElapsedLifeSpan;
            timer.Start();
        }

        public TempMessage(IMessage message, double lifeSpan)
        {
            this.message = message;
            timer = new Timer(lifeSpan * 1000);
            timer.AutoReset = false;
            timer.Elapsed += ElapsedLifeSpan;
            timer.Start();
        }

        private void ElapsedLifeSpan(object sender, ElapsedEventArgs e)
        {
            timer.Dispose();
            message.DeleteAsync().GetAwaiter().GetResult();
            Data.tempMessages.Remove(this);
        }
    }
}
