using Discord.Commands;
using Discord.WebSocket;
using IwuhSelfbot.Commands.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands
{
    public class SelfbotCommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IDependencyMap _map;
        private ulong _userId;

        public async Task InstallAsync(IDependencyMap map)
        {
            _client = map.Get<DiscordSocketClient>();
            _userId = _client.CurrentUser.Id;

            _commands = new CommandService();
            map.Add(_commands);
            _map = map;

            AddServices(_map);
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

            _client.MessageReceived += HandleCommand;
        }

        private void AddServices(IDependencyMap map)
        {
            map.Add(new SelfbotConfigService());
        }

        private async Task HandleCommand(SocketMessage msg)
        {
            // If userMessage is null, then the message isn't from a user and should be ignored.
            var userMessage = msg as SocketUserMessage;
            if (userMessage == null) return;
            // Only allow commands to be executed by the person running the bot.
            if (userMessage.Author.Id != _userId) return;

            int argPos = 0;
            if (userMessage.HasStringPrefix(_map.Get<SelfbotConfigService>().Prefix, ref argPos))
            {
                var context = new CommandContext(_client, userMessage);
                var result = await _commands.ExecuteAsync(context, argPos, dependencyMap: _map);

                if (!result.IsSuccess)
                {
                    switch (result.Error)
                    {
                        case CommandError.BadArgCount:
                            await msg.Channel.SendMessageAsync("Error: bad argument count. Check the command help.");
                            break;
                        case CommandError.Exception:
                            await msg.Channel.SendMessageAsync($"Error: an exception occurred.\n{result.ErrorReason}\nPlease contact Iwuh#6351 if this happens again.");
                            break;
                        case CommandError.ParseFailed:
                            await msg.Channel.SendMessageAsync($"Error: argument parsing failed.\n{result.ErrorReason}\nCheck the command help.");
                            break;
                        case CommandError.UnknownCommand:
                            await msg.Channel.SendMessageAsync("Error: unknown command. Check the help for a list of commands.");
                            break;
                        case CommandError.UnmetPrecondition:
                            await msg.Channel.SendMessageAsync($"Error: a precondition was not met.\n{result.ErrorReason}");
                            break;
                    }
                }
            }
        }
    }
}
