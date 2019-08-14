using System.Threading.Tasks;
using Discord.WebSocket;

namespace BusinessLogic.Services.Interfaces
{
    public interface IDiscordSocketClientService
    {
        Task DiscordSocketClientOnUserVoiceStateUpdated(SocketUser socketUser, SocketVoiceState socketVoiceState1, SocketVoiceState socketVoiceState2);
    }
}