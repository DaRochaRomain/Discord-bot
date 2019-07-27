using System;
using System.IO;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Commands
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly IAudioService _audioService;
        private readonly IConfiguration _configuration;

        public AudioModule(IAudioService audioService, IConfiguration configuration)
        {
            _audioService = audioService;
            _configuration = configuration;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play(params string[] titles)
        {
            try
            {
                var audiosFolder = _configuration["AudiosFolder"];

                if (Context.User is IGuildUser guildUser)
                {
                    var voiceChannel = guildUser.VoiceChannel;

                    if (voiceChannel != null)
                        using (var audioClient = await voiceChannel.ConnectAsync())
                        {
                            foreach (var title in titles)
                            {
                                var path = Path.Combine(audiosFolder, $"{title}.mp3");

                                if (File.Exists(path))
                                    await _audioService.SendAsync(audioClient, path);
                            }
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}