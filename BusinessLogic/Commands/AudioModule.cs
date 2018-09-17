using System;
using System.Collections.Generic;
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
        public async Task Play(params string[] audioNames)
        {
            try
            {
                if (audioNames != null && Context.User is IGuildUser guildUser)
                {
                    var audiosNotFound = new List<string>();
                    var filePaths = new List<string>();
                    var filesNotFound = new List<string>();

                    foreach (var audioName in audioNames)
                    {
                        var filePath = _configuration[audioName];

                        if (string.IsNullOrWhiteSpace(filePath))
                            audiosNotFound.Add(audioName);
                        else
                        {
                            if (File.Exists(filePath))
                                filePaths.Add(filePath);
                            else
                                filesNotFound.Add(filePath);
                        }
                    }

                    if (audiosNotFound.Count > 0)
                    {
                        foreach (var audioNotFound in audiosNotFound)
                            await ReplyAsync($"Audio not found : {audioNotFound}");
                    }

                    if (filesNotFound.Count > 0)
                    {
                        foreach (var fileNotFound in filesNotFound)
                            await ReplyAsync($"File not found : {fileNotFound}");
                    }

                    if (filePaths.Count > 0)
                    {
                        var voiceChannel = guildUser.VoiceChannel;

                        try
                        {
                            var audioClient = await voiceChannel.ConnectAsync();

                            foreach (var filePath in filePaths)
                                await _audioService.SendAsync(audioClient, filePath);

                            await audioClient.StopAsync();
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
