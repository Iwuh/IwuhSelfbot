using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Modules
{
    public class FunModule : ModuleBase
    {
        [Command("me")]
        [Summary("Shows the input as if it's an action you're performing.")]
        public async Task MeCommand([Remainder, Summary("The action you're performing.")] string input)
        {
            Color embedColour = Context.User.GetHighestRoleColor(Context.Guild);

            // Create the embed to use as the message.
            EmbedBuilder meEmbed = new EmbedBuilder()
                .WithColor(embedColour)
                .WithDescription($"{Format.Bold((Context.User as SocketGuildUser)?.Nickname ?? Context.User.Username)} {Format.Italics(input)}");

            // Delete the command message.
            await Context.Message.DeleteAsync();
            // Send the "me" message.
            await ReplyAsync(string.Empty, embed: meEmbed);
        }
    }
}
