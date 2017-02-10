using Discord.Commands;
using IwuhSelfbot.Commands.Entities;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Services
{
    public class EvalService
    {
        private ScriptOptions _evalOptions;

        public EvalService()
        {
            _evalOptions = ScriptOptions.Default
                .WithReferences(typeof(object).GetTypeInfo().Assembly, typeof(Enumerable).GetTypeInfo().Assembly, typeof(Discord.Embed).GetTypeInfo().Assembly,
                                typeof(Discord.WebSocket.DiscordShardedClient).GetTypeInfo().Assembly, typeof(Discord.Commands.ModuleBase).GetTypeInfo().Assembly)
                .WithImports("System", "System.Linq", "System.Text", "System.Threading.Tasks", "System.Collections.Generic", "System.Reflection", 
                             "Discord", "Discord.WebScocket", "Discord.Commands");
        }

        public async Task<EvalResult> EvaluateAsync(string input, SocketCommandContext context)
        {
            bool successful;
            string output;
            try
            {
                object result = await CSharpScript.EvaluateAsync(input, options: _evalOptions, globals: new EvalGlobals(context.Client, context));

                // If we get this far, the evaluation succeeded.
                successful = true;
                output = result?.ToString() ?? "null";
            }
            catch (CompilationErrorException e)
            {
                // If an error occurs while compiling, return the error message.
                successful = false;
                output = e.Message;
            }

            return new EvalResult(successful, output);
        }
    }
}
