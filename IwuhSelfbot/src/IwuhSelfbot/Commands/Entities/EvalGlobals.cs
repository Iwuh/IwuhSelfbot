using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Entities
{
    public class EvalGlobals
    {
        public DiscordSocketClient Client { get; private set; }
        public CommandContext Context { get; private set; }

        public EvalGlobals(DiscordSocketClient client, CommandContext context)
        {
            Client = client;
            Context = context;
        }
    }
}
