﻿using System;
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
using System.Windows.Forms;

namespace InfiniBot
{
    public class Bot
    {
        public static Random rng = new Random();

        public DiscordSocketClient client;
        public CommandService commands;
        private IServiceProvider _services;

        public async Task RunBotAsync(string token)
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance
            });

            commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();

            //Event subscriptions
            client.Log += Program.MainForm.Log;
            commands.Log += Program.MainForm.Log;
            commands.CommandExecuted += CommandExecuted;
            client.UserJoined += UserJoined;
            client.LoggedIn += Program.MainForm.BotLoggedIn;
            client.LoggedOut += Program.MainForm.BotLoggedOut;

            await RegisterCommandsAsync();

            try
            {
                await client.LoginAsync(TokenType.Bot, token);
                await client.SetGameAsync($"{Data.BOT_PREFIX}help for commands");

                await client.StartAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Something went wrong while trying to connect to the bot account.\n\nException:\n{e}", "ERROR: Could not connect");
            }
        }

        public async Task StopBotAsync()
        {
            if (client != null)
            {
                if (client.LoginState == LoginState.LoggedIn)
                {
                    await client.LogoutAsync();
                    await client.StopAsync();
                }
            }
        }

        public bool IsActive()
        {
            if (client != null)
            {
                return client.LoginState == LoginState.LoggedIn;
            }
            return false;
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
            client.MessageReceived += HandleMessageAsync;

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

            if (message.HasStringPrefix(Data.BOT_PREFIX, ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))
            {
                SocketCommandContext context = new SocketCommandContext(client, message);

                IResult result = await commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
