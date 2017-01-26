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
            var guildUser = Context.User as SocketGuildUser;
            Color embedColour; // Colour has a u, fight me irl
            if (guildUser == null)
            {
                // If we get here it means we're in a DM, so just use the default colour.
                embedColour = Color.Default;
            }
            else
            {
                // Convert each role ID to a role object, then order them by position in the hierarchy (highest first).
                var roles = guildUser.RoleIds.Select(r => Context.Guild.GetRole(r)).OrderByDescending(r => r.Position);

                // If none of the roles have a colour...
                if (!roles.Any(r => !r.Color.Equals(Color.Default)))
                {
                    // ...use the default.
                    embedColour = Color.Default;
                }
                else
                {
                    // Pick the highest role that has a colour, and use it.
                    embedColour = roles.FirstOrDefault(r => !r.Color.Equals(Color.Default)).Color;
                }
            }

            // Create the embed to use as the message.
            EmbedBuilder meEmbed = new EmbedBuilder()
                .WithColor(embedColour)
                .WithDescription($"{Format.Bold(guildUser?.Nickname ?? Context.User.Username)} {Format.Italics(input)}");

            // Delete the command message.
            await Context.Message.DeleteAsync();
            // Send the "me" message.
            await ReplyAsync(string.Empty, embed: meEmbed);
        }
    }
}
