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

        [Command("quote")]
        [Summary("Shows a message that somebody else wrote. Does not quote embeds. Note: due to a user account restriction, certain messages may not be able to be quoted.")] 
        public async Task QuoteMessage([Summary("The ID of the message to quote.")] ulong id)
        {
            // Users can't access the get message endpoint, but they _can_ access the get messages endpoint.
            var messages = await Context.Channel.GetMessagesAsync(id, Direction.Around, limit: 3).Flatten();
            var message = messages.First(m => m.Id == id);

            // Don't quote any message that's only an embed with no text.
            if (message.Content == string.Empty && message.Embeds.Count > 0)
            {
                await ReplyAsync("The quoted message must have text.");
                return;
            }

            // Get the image attachment, if there is one.
            IAttachment image = message.Attachments.FirstOrDefault(a =>
            {
                string lower = a.Filename.ToLower();
                // Check if it ends with any of these common image file extensions.
                return lower.EndsWith(".png") || lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".gif");
            });

            EmbedBuilder quoteBuilder = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder() { IconUrl = message.Author.AvatarUrl, Name = message.Author.ToString() })
                .WithColor(message.Author.GetHighestRoleColor(Context.Guild))
                .WithDescription(message.Content)
                .WithTimestamp(message.Timestamp);

            if (image != null)
            {
                quoteBuilder.WithImageUrl(image.Url);
            }

            await Context.Message.DeleteAsync();
            await ReplyAsync(string.Empty, embed: quoteBuilder);
        }
    }
}
