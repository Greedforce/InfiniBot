using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;

namespace InfiniBot
{
    public class Bot
    {
        public static Random rng = new Random();

        private DiscordSocketClient _client;
        public static CommandService commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance
            });

            commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(commands)
                .BuildServiceProvider();

            //Event subscriptions
            _client.Log += Program.MainForm.Log;
            commands.Log += Program.MainForm.Log;
            commands.CommandExecuted += CommandExecuted;
            _client.UserJoined += UserJoined;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Data.TEST_BOT_TOKEN);
            await _client.SetGameAsync($"{Data.BOT_PREFIX}help for commands");

            await _client.StartAsync();
        }

        public async Task StopBotAsync()
        {
            if (_client != null)
            {
                if (_client.LoginState == LoginState.LoggedIn)
                {
                    await _client.LogoutAsync();
                    await _client.StopAsync();
                }
            }
        }

        private Task UserJoined(SocketGuildUser user)
        {
            Embed embed = Data.GetJoinEmbed(user.Guild);
            user.SendMessageAsync("", false, embed);
            return Task.CompletedTask;
        }

        private Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            string name = "Unknown command";
            if (command.IsSpecified)
            {
                if (command.Value.Module.Group != null)
                {
                    name = command.Value.Module.Group + " ";
                }
                name = command.Value.Name;
            }

            return Program.MainForm.Log(new LogMessage(LogSeverity.Debug, "Command", name + " executed successfully", null));
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleMessageAsync;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleMessageAsync(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;

            if (message.HasStringPrefix(Data.BOT_PREFIX, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                SocketCommandContext context = new SocketCommandContext(_client, message);

                IResult result = await commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
