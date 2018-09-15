using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.Commands;

namespace BusinessLogic.Commands
{
    [Group("!sbb")]
    public class AudioModule : ModuleBase
    {
        private readonly IAudioService _audioService;

        public AudioModule(IAudioService audioService)
        {
            _audioService = audioService;
        }

        [Command("test")]
        public async Task Test()
        {
            if (Context.User is IGuildUser guildUser)
            {
                var voiceChannel = guildUser.VoiceChannel;

                await voiceChannel.ConnectAsync(async audioClient =>
                {
                    await audioClient.SetSpeakingAsync(true);
                    await _audioService.SendAsync(audioClient, @"C:\Users\daroc\Downloads\100Hz_44100Hz_16bit_05sec.wav");
                    await audioClient.StopAsync();
                });
            }
        }
    }
}
