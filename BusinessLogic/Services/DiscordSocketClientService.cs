using System.IO;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Services
{
    public class DiscordSocketClientService : IDiscordSocketClientService
    {
        private readonly IAudioService _audioService;
        private readonly IConfiguration _configuration;

        public DiscordSocketClientService(
            IConfiguration configuration,
            IAudioService audioService)
        {
            _configuration = configuration;
            _audioService = audioService;
        }

        public async Task DiscordSocketClientOnUserVoiceStateUpdated(SocketUser socketUser, SocketVoiceState socketVoiceState1, SocketVoiceState socketVoiceState2)
        {
//            if (socketUser is IGuildUser guildUser)
//                await PlayWesh(guildUser);
        }

//        private async Task PlayWesh(IVoiceState voiceState)
//        {
//            var audiosFolder = _configuration["AudiosFolder"];
//            var file = Path.Combine(audiosFolder, "ratata.mp3");
//
//            if (File.Exists(file))
//                await _audioService.PlayFiles(voiceState, file);
//        }
    }
}