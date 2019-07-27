using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace BusinessLogic.Commands
{
    public class DebugModule : ModuleBase<SocketCommandContext>
    {
        [Command("debug", RunMode = RunMode.Async)]
        public async Task Debug()
        {
        }

        [Command("ping", RunMode = RunMode.Async)]
        public async Task Pong()
        {
            try
            {
                await ReplyAsync("pong");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}