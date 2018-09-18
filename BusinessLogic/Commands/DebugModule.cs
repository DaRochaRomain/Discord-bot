using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BusinessLogic.Commands
{
    public class DebugModule : BaseCommandModule
    {
        [Command("debug")]
        public async Task Debug(CommandContext commandContext)
        {
        }

        [Command("ping")]
        public async Task Pong(CommandContext commandContext)
        {
            try
            {
                await commandContext.RespondAsync("pong");
            }
            catch (Exception e)
            {
                await commandContext.RespondAsync(e.ToString());
            }
        }
    }
}
