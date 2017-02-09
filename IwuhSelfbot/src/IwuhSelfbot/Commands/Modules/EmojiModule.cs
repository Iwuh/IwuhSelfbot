using Discord.Commands;
using IwuhSelfbot.Commands.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Modules
{
    [Group("emoji")]
    public class EmojiModule : ModuleBase
    {
        private EmojiService _emojis;

        public EmojiModule(EmojiService emoji)
        {
            _emojis = emoji;
        }

        [Command("add")]
        [Summary("Adds an emoji to the list. The name will be replaced by the content when surrounded by colons.")]
        public async Task AddEmoji([Summary("The name of the emoji (don't include the colons).")] string name, [Remainder, Summary("What to replace the name with")] string content)
        {
            await _emojis.AddEmojiAsync(name, content);
        }

        [Command("remove")]
        [Summary("Removes an emoji from the list.")]
        public async Task RemoveEmoji([Summary("The name of the emoji to remove.")] string name)
        {
            if (await _emojis.RemoveEmojiAsync(name))
            {
                await Context.Message.ModifyAsync(m => m.Content = $"Emoji {name} successfully removed.");
            }
            else
            {
                await Context.Message.ModifyAsync(m => m.Content = $"Emoji {name} could not be removed.");
            }
        }

        [Command("list")]
        [Summary("Lists all emojis currently in the bot.")]
        public async Task ListEmojis()
        {
            await Context.Message.ModifyAsync(m => m.Content = _emojis.ListEmojis());
        }
    }
}
