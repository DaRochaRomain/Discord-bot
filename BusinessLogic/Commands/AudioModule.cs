using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Commands
{
    [Group("!sbb")]
    public class AudioModule : ModuleBase
    {
        private readonly IAudioService _audioService;
        private readonly IConfiguration _configuration;

        public AudioModule(
            IAudioService audioService,
            IConfiguration configuration)
        {
            _audioService = audioService;
            _configuration = configuration;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play(string audioName)
        {
            try
            {
                var filePath = _configuration[audioName];

                if (filePath != null && Context.User is IGuildUser guildUser)
                {
                    var voiceChannel = guildUser.VoiceChannel;
                    var audioClient = await voiceChannel.ConnectAsync();

                    await _audioService.SendAsync(audioClient, filePath);
                    await audioClient.StopAsync();
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }

        [Command("playlist")]
        public async Task Playlist()
        {
            try
            {
                var keys = _configuration
                    .AsEnumerable()
                    .Select(e => e.Key)
                    .OrderBy(e => e, StringComparer.OrdinalIgnoreCase);
                var playlist = keys.Aggregate((s1, s2) => $"{s1}{Environment.NewLine}{2}");

                await ReplyAsync(playlist);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
    }
}
