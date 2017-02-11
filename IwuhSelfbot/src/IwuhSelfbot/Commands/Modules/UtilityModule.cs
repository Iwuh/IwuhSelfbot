using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IwuhSelfbot.Commands.Entities;
using IwuhSelfbot.Commands.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Modules
{
    public class UtilityModule : ModuleBase
    {
        private EvalService _evaluator;
        private SelfbotConfigService _config;

        public UtilityModule(EvalService eval, SelfbotConfigService conf)
        {
            _evaluator = eval;
            _config = conf;
        }

        [Command("eval")]
        [Summary("[Advanced] Evaluate a C# expression using the Roslyn Scripting API.")]
        public async Task EvaluateExpression([Remainder, Summary("The C# expression to evaluate.")] string expr)
        {
            EvalResult result = await _evaluator.EvaluateAsync(expr, Context);

            if (result.IsSuccessful)
            {
                await Context.Message.ModifyAsync(m => m.Content = $"Input:\n```{expr}```\nOutput:\n```{result.Output}```");
            }
            else
            {
                await Context.Message.ModifyAsync(m => m.Content = $"Exception:\n```{result.Output}```");
            }
        }

        [Command("time")]
        [Summary("Gets your current time.")]
        public async Task CurrentTime()
        {
            var now = DateTimeOffset.Now;
            var converted = TimeZoneInfo.ConvertTime(now, _config.Timezone);

            // Cast the user to a socket guild user and get the nickname. If that's null, use the username instead.
            string nameToUse = (Context.User as SocketGuildUser)?.Nickname ?? Context.User.Username;

            await Context.Message.ModifyAsync(m => m.Content = $"The current time for {Format.Bold(nameToUse)} is `{converted.TimeOfDay.ToString(@"hh\:mm")}`.");
        }
    }
}
