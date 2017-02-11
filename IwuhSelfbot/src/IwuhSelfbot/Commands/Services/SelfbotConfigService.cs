using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Services
{
    public class SelfbotConfigService
    {
        public string Token { get; private set; }
        public string Prefix { get; private set; }
        public ulong UserId { get; private set; }
        public TimeZoneInfo Timezone { get; private set; }

        /// <summary>
        /// Creates a new instance, reading the token and prefix from the config file.
        /// </summary>
        public SelfbotConfigService()
        {
            JObject config = JObject.Parse(File.ReadAllText(@".\SelfbotConfig.json"));
            Token = (string)config["Token"];
            Prefix = (string)config["Prefix"];
            UserId = (ulong)config["ID"];
            Timezone = ParseTimezone((string)config["Timezone"]);
        }

        private TimeZoneInfo ParseTimezone(string timezone)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
    }
}
