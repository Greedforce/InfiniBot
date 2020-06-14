using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace InfiniBot
{
    [Group("Set")]
    public class BotPropertyModule : ModuleBase<SocketCommandContext>
    {
        [Command("Nickname", RunMode = RunMode.Async)]
        [Summary("Changes the bots nickname.")]
        [RequireUserPermission(GuildPermission.ManageNicknames, Group = "user")]
        [RequireOwner(Group = "user")]
        public async Task ChangeBotNicknameAsync(
            [Summary("The new nickname.")]
            [Example("DiscordBot1337")]
            [Remainder]
            string nickName)
        {
            await Context.Message.DeleteAsync();
            await Context.Guild.GetUser(Context.Client.CurrentUser.Id).ModifyAsync(u => u.Nickname = nickName);
            IMessage m = await ReplyAsync(
                embed: new EmbedBuilder()
                .WithTitle("Nickname Changed")
                .WithDescription($"My nickname on this server has been changed to {nickName}")
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("Username", RunMode = RunMode.Async)]
        [Summary("Changes the bots username.")]
        [RequireOwner]
        public async Task ChangeBotUsernameAsync(
            [Summary("The new username.")]
            [Example("DiscordBot1337")]
            [Remainder]
            string userName)
        {
            await Context.Message.DeleteAsync();
            await Context.Client.CurrentUser.ModifyAsync(u => u.Username = userName);
            IMessage m = await ReplyAsync(
                embed: new EmbedBuilder()
                .WithTitle("Username Changed")
                .WithDescription($"My username has been changed to {userName}")
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("Game", RunMode = RunMode.Async)]
        [Summary("Changes the bots game.")]
        [RequireOwner]
        public async Task ChangeBotGameAsync(
            [Summary("The new game.")]
            [Example("Overwatch")]
            [Remainder]
            string game)
        {
            await Context.Message.DeleteAsync();
            await Context.Client.SetGameAsync(game);
            IMessage m = await ReplyAsync(
                embed: new EmbedBuilder()
                .WithTitle("Game Changed")
                .WithDescription($"My game has been changed to {game}")
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }

        [Command("Avatar", RunMode = RunMode.Async)] // Not working
        [Summary("Changes the bots avatar.")]
        [RequireOwner]
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
            IMessage m = await ReplyAsync(
                embed: new EmbedBuilder()
                .WithTitle("Avatar Changed")
                .WithDescription($"My avatar has been changed")
                .WithImageUrl(avatar.Url)
                .WithAutoDeletionFooter()
                .Build());
            await Task.Delay(Data.MESSAGE_DELETE_DELAY * 1000);
            await m.DeleteAsync();
        }
    }
}
