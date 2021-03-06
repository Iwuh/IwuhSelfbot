using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Services
{
    public class EmojiService
    {
        private Dictionary<string, string> _emojis;
        private ulong _userId;

        /// <summary>
        /// Creates a new instance of the server and hooks up the message handler.
        /// </summary>
        /// <param name="client">The client that the message handler should be subscribed to.</param>
        /// <param name="id">The ID of the selfbot user.</param>
        public EmojiService(DiscordSocketClient client, ulong id)
        {
            _emojis = new Dictionary<string, string>();
            _userId = id;

            client.MessageReceived += ReplaceEmojis;
        }

        /// <summary>
        /// Loads custom emojis from a file into the dictionary.
        /// </summary>
        public async Task LoadEmojisAsync()
        {
            //if (!File.Exists(@".\emojis.json"))
            //{
            //    using (FileStream emojiStream = File.Open(@".\emojis.json", FileMode.Create))
            //    {
            //        const string json = "{\n\n}";

            //        byte[] bytes = Encoding.UTF8.GetBytes(json);

            //        await emojiStream.WriteAsync(bytes, 0, bytes.Length);
            //    }
            //}

            using (FileStream emojiStream = File.Open(@".\emojis.json", FileMode.Open))
            {
                var unicode = Encoding.UTF8;

                byte[] emojiBytes = new byte[emojiStream.Length];
                await emojiStream.ReadAsync(emojiBytes, 0, (int)emojiStream.Length);

                string emojiJson = unicode.GetString(emojiBytes);
                Dictionary<string, string> emojiDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(emojiJson);

                foreach (var pair in emojiDict)
                {
                    try
                    {
                        _emojis.Add(pair.Key, pair.Value);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Warning: an emoji was loaded from emojis.json that was already in the dictionary.");
                    }
                }
            }
        }

        /// <summary>
        /// Adds an emoji to the list.
        /// </summary>
        /// <param name="name">The string to replace when surrounded by colons.</param>
        /// <param name="value">What to replace it with.</param>
        public async Task AddEmojiAsync(string name, string value)
        {
            _emojis.Add(name, value);
            await SaveEmojisAsync();
        }

        /// <summary>
        /// Removes an emoji from the list by name.
        /// </summary>
        /// <param name="name">The name of the emoji to remove.</param>
        /// <returns>Whether or not the removal option completed successfully.</returns>
        public async Task<bool> RemoveEmojiAsync(string name)
        {
            if (_emojis.Remove(name))
            {
                await SaveEmojisAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Lists all saved emojis.
        /// </summary>
        public string ListEmojis()
        {
            if (_emojis.Count == 0)
            {
                return "There are no emojis saved.";
            }
            else
            {
                var emojiListBuilder = new StringBuilder();

                foreach (var pair in _emojis)
                {
                    emojiListBuilder.AppendLine($"`{pair.Key}` - {pair.Value}");
                }

                return emojiListBuilder.ToString();
            }
        }

        private async Task SaveEmojisAsync()
        {
            using (FileStream emojiStream = File.Open(@".\emojis.json", FileMode.Truncate))
            {
                var unicode = Encoding.UTF8;

                string serialized = JsonConvert.SerializeObject(_emojis);
                byte[] emojiBytes = unicode.GetBytes(serialized);

                await emojiStream.WriteAsync(emojiBytes, 0, emojiBytes.Length);
            }
        }

        private async Task ReplaceEmojis(SocketMessage message)
        {
            // Don't do anything if it isn't a user message.
            var userMessage = message as SocketUserMessage;
            if (userMessage == null) return;
            // Skip messages that aren't by the selfbot user.
            if (userMessage.Author.Id != _userId) return;

            string replacement = userMessage.Content;
            foreach (string name in _emojis.Keys)
            {
                replacement = replacement.Replace($":{name}:", _emojis[name]);
            }

            if (replacement != userMessage.Content)
            {
                await userMessage.ModifyAsync(m => m.Content = replacement);
            }
        }
    }
}
