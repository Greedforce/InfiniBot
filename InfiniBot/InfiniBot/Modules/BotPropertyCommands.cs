using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace InfiniBot
{
    [Group("Set")]
    public class BotPropertyCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Nickname", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ChangeNickname, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task ChangeBotNicknameAsync(
            [Remainder]
            [Example("DiscordBot1337")]
            string nickName)
        {
            await Context.Message.DeleteAsync();
            await Context.Guild.GetUser(Context.Client.CurrentUser.Id).ModifyAsync(u => u.Nickname = nickName);
            IMessage m = await ReplyAsync(embed: Data.GetFeedbackEmbed().WithTitle("Nickname Changed").WithDescription($"My nickname on this server has been changed to {nickName}").Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("Username", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task ChangeBotUsernameAsync(
            [Remainder]
            [Example("DiscordBot1337")]
            string userName)
        {
            await Context.Message.DeleteAsync();
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = userName);
            IMessage m = await ReplyAsync(embed: Data.GetFeedbackEmbed().WithTitle("Username Changed").WithDescription($"My username has been changed to {userName}").Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("Game", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task ChangeBotGameAsync(
            [Remainder]
            [Example("Overwatch")]
            string game)
        {
            await Context.Message.DeleteAsync();
            await Context.Client.SetGameAsync(game);
            IMessage m = await ReplyAsync(embed: Data.GetFeedbackEmbed().WithTitle("Game Changed").WithDescription($"My game has been changed to {game}").Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("Avatar", RunMode = RunMode.Async)]
        [RequireOwner] // Not working
        public async Task ChangeBotGameAsync()
        {
            Attachment avatar = Context.Message.Attachments.FirstOrDefault();
            await Context.Message.DeleteAsync();
            Stream newAvatar = new MemoryStream();
            byte[] imageData = null;

            using (var wc = new WebClient())
                imageData = wc.DownloadData(avatar.Url);
            newAvatar = new MemoryStream(imageData);
            await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(newAvatar));
            IMessage m = await ReplyAsync(embed: Data.GetFeedbackEmbed().WithTitle("Avatar Changed").WithDescription($"My avatar has been changed").WithImageUrl(avatar.Url).Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }
    }
}
