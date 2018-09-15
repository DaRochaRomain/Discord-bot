using System.Threading.Tasks;
using Discord.Commands;

namespace BusinessLogic.Commands
{
    [Group("!sbb")]
    public class PingModule : ModuleBase
    {
        [Command("Ping", RunMode = RunMode.Sync)]
        public async Task Pong()
        {
            await ReplyAsync("Pong");
        }
    }
}
