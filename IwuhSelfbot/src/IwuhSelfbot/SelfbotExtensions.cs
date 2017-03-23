using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot
{
    public static class SelfbotExtensions
    {
        public static Color GetHighestRoleColor(this IUser user, IGuild guild)
        {
            var guildUser = user as SocketGuildUser;
            Color embedColour; // Colour has a u, fight me irl
            if (guildUser == null)
            {
                // If we get here it means we're in a DM, so just use the default colour.
                embedColour = Color.Default;
            }
            else
            {
                // Order the roles by position in the hierarchy (highest first).
                var roles = guildUser.Roles.OrderByDescending(r => r.Position);

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

            return embedColour;
        }
    }
}
