using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace BusinessLogic.Commands
{
    [Group("!sbb")]
    public class DebugModule : ModuleBase
    {
        [Command("debug", RunMode = RunMode.Async)]
        public async Task Debug()
        {
            try
            {
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }

        [Command("ping", RunMode = RunMode.Async)]
        public async Task Pong()
        {
            try
            {
                await ReplyAsync("Pong");
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
    }
}
