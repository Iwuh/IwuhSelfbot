using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IwuhSelfbot.Commands;
using IwuhSelfbot.Commands.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private SelfbotCommandHandler _handler;
        private DependencyMap _map;

        public static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                // Set only general info to be logged.
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 1000
            });

            _client.Log += message =>
            {
                Console.WriteLine(message.ToString());
                // Represents a completed task for methods that have to return Task but don't do anything asychronously.
                return Task.CompletedTask;
            };

            // Initialise the dependency map that will be passed around and add the client to it.
            _map = new DependencyMap();
            _map.Add(_client);

            // Initialise the command handler, and install modules, services, etc.
            _handler = new SelfbotCommandHandler();
            await _handler.InstallAsync(_map);

            // Get the token from the config service, login, and connect.
            string token = _map.Get<SelfbotConfigService>().Token;
            await _client.LoginAsync(TokenType.User, token);
            await _client.StartAsync();

            // Delay asynchronously for an infinite amount of time, preventing the program from exiting but not blocking the thread.
            await Task.Delay(-1);
        }
    }
}
