using System.Threading.Tasks;
using Discord.Commands;

namespace BusinessLogic.Commands
{
    [Group("!sbb")]
    public class PingModule : ModuleBase
    {
        [Command("Ping")]
        public async Task Pong()
        {
            await ReplyAsync("Pong");
        }
    }
}
