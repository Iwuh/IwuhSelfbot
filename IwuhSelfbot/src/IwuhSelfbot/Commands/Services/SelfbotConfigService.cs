using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            if (timezone != string.Empty)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(timezone);
                }
                catch (Exception e) when (e.GetType().ToString() == "System.TimeZoneNotFoundException")
                {
                    /*
                     * So, a bit of explanation. In .NET Core 1.1, TimeZoneNotFoundException isn't exposed publicly.
                     * However, we still need to catch it. The workaround we use here is catching a generic exception,
                     * then filtering to only catch TimeZoneNotFoundException by getting the type, converting it to a
                     * string, then comparing it to the string literal ToString() will return if e is a TimeZoneNotFoundException.
                     */
                    Console.WriteLine("Invalid timezone, using system default.");
                }
            }
            else
            {
                Console.WriteLine("No timezone specified, using system default.");
            }

            return TimeZoneInfo.Local;   
        }
    }
}
