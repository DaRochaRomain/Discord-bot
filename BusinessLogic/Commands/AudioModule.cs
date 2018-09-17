using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private async Task<List<string>> ParseAudioNames(params string[] audioNames)
        {
            var errorStringBuilder = new StringBuilder();
            var filePaths = new List<string>();

            foreach (var audioName in audioNames)
            {
                var filePath = _configuration[audioName];

                if (string.IsNullOrWhiteSpace(filePath))
                    errorStringBuilder.AppendLine($"Audio not found : {audioName}");
                else
                {
                    if (File.Exists(filePath))
                        filePaths.Add(filePath);
                    else
                        errorStringBuilder.AppendLine($"File not found : {filePath}");
                }
            }

            if (errorStringBuilder.Length > 0)
                await ReplyAsync(errorStringBuilder.ToString());

            return filePaths;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play(params string[] audioNames)
        {
            try
            {
                if (audioNames != null && Context.User is IGuildUser guildUser)
                {
                    var filePaths = await ParseAudioNames(audioNames);

                    if (filePaths.Count > 0)
                    {
                        var voiceChannel = guildUser.VoiceChannel;

                        try
                        {
                            //var audioClient = await voiceChannel.ConnectAsync();

                            //foreach (var filePath in filePaths)
                            //    await _audioService.SendAsync(audioClient, filePath);

                            //await audioClient.StopAsync();

                            //Fixes the fact that opening multiple times successively the stream of one audio client to send audio doesn't work
                            foreach (var filePath in filePaths)
                            {
                                var audioClient = await voiceChannel.ConnectAsync();

                                await _audioService.SendAsync(audioClient, filePath);
                                await audioClient.StopAsync();
                            }
                        }
                        catch (TaskCanceledException)
                        {
                            //Ignored
                        }
                    }
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
