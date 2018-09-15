using System;
using System.IO;
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

                if (!string.IsNullOrEmpty(filePath) && Context.User is IGuildUser guildUser)
                {
                    if (File.Exists(filePath))
                    {
                        var voiceChannel = guildUser.VoiceChannel;
                        var audioClient = await voiceChannel.ConnectAsync();

                        await _audioService.SendAsync(audioClient, filePath);
                        await audioClient.StopAsync();
                    }
                    else
                        await ReplyAsync($"File not found : {filePath}");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }

        [Command("playlist", RunMode = RunMode.Async)]
        public async Task Playlist()
        {
            try
            {
                var playlist = _configuration
                    .AsEnumerable()
                    .Select(e => $"!sbb play {e.Key}")
                    .OrderBy(e => e, StringComparer.OrdinalIgnoreCase)
                    .Aggregate((s1, s2) => $"{s1}{Environment.NewLine}{s2}");
                var embedBuilder = new EmbedBuilder();

                embedBuilder.AddField("Playlist", playlist);
                await Context.Channel.SendMessageAsync(string.Empty, false, embedBuilder.Build());
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
    }
}
