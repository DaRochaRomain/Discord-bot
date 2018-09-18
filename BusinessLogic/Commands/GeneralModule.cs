using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;

namespace BusinessLogic.Commands
{
    public class GeneralModule : BaseCommandModule
    {
        [Command("stop")]
        public async Task Stop(CommandContext commandContext)
        {
            try
            {
                var voiceNext = commandContext.Client.GetVoiceNext();
                var voiceNextConnection = voiceNext.GetConnection(commandContext.Member.Guild);

                voiceNextConnection?.Disconnect();
                ;
            }
            catch (Exception e)
            {
                await commandContext.RespondAsync(e.ToString());
            }
        }
    }
}
