using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Modules
{
    public class SettingsModule : ModuleBase
    {
        public async Task SetGame([Remainder, Summary("The text to set as your currently playing game.")] string game)
        {
            await (Context.Client as DiscordSocketClient).SetGameAsync(game);
            await Context.Message.ModifyAsync(m => m.Content = $"Game set to `{game}`. This will appear for everybody else, but you will not see it in your own client.");
        }
    }
}
