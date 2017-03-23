using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                .WithAuthor(new EmbedAuthorBuilder() { IconUrl = message.Author.GetAvatarUrl(), Name = message.Author.ToString() })
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

        [Command("poll")]
        [Summary("Creates a pool that people can respond to via reactions. Usage: `question|option 1|option 2|etc...`")]
        [RequireBotPermission(ChannelPermission.AddReactions)]
        public async Task CreatePoll([Remainder, Summary("Creates a message with a question and several reaction options.")] string input)
        {
            const int REGIONAL_INDICATOR_A = 0x1F1E6;
            const string THUMBS_UP = "👍";
            const string THUMBS_DOWN = "👎";
            const string CLIPBOARD = "📋";

            var split = input.Split('|');

            if (split.Length == 1)
            {
                await Context.Message.ModifyAsync(m => m.Content = $"{CLIPBOARD}{Format.Italics(split[0])}");
                await Context.Message.AddReactionAsync(THUMBS_UP);
                await Context.Message.AddReactionAsync(THUMBS_DOWN);
            }
            else
            {
                var pollBuilder = new StringBuilder($"{CLIPBOARD}{Format.Italics(split[0])}\n");
                var emojiList = new List<string>();

                for (int i = 1; i < split.Length; i++)
                {
                    string currentEmoji = char.ConvertFromUtf32(REGIONAL_INDICATOR_A + i - 1);
                    pollBuilder.AppendLine($"{currentEmoji} {split[i]}");
                    emojiList.Add(currentEmoji);
                }

                await Context.Message.ModifyAsync(m => m.Content = pollBuilder.ToString());
                foreach (string emoji in emojiList)
                {
                    await Context.Message.AddReactionAsync(emoji);
                }
            }
        }
    }
}
