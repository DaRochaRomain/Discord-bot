using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BusinessLogic.Commands
{
    [Group("!sbb")]
    public class GeneralModule : ModuleBase
    {
        [Command("stop")]
        public async Task Stop()
        {
            try
            {
                if (Context.User is IGuildUser guildUser)
                {
                    var voiceChannel = guildUser.VoiceChannel;

                    await voiceChannel.DisconnectAsync();
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
    }
}
