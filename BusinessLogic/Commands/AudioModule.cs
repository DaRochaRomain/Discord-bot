using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Utils;
using BusinessLogic.Utils.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Commands
{
    public class AudioModule : BaseCommandModule
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

        private List<string> GetFilePaths(IEnumerable<string> audioNames, out List<string> audiosNotFound)
        {
            var audiosFolder = _configuration["audiosfolder"];
            var filePaths = new List<string>();
            audiosNotFound = new List<string>();

            foreach (var audioName in audioNames)
            {
                var filePath = $"{Path.Combine(audiosFolder, audioName)}.mp3";

                if (File.Exists(filePath))
                    filePaths.Add(filePath);
                else
                    audiosNotFound.Add(audioName);
            }

            return filePaths;
        }

        [Command("play")]
        public async Task Play(CommandContext commandContext, params string[] audioNames)
        {
            try
            {
                if (audioNames != null)
                {
                    var filePaths = GetFilePaths(audioNames, out var audiosNotFound);

                    if (audiosNotFound.Count > 0)
                    {
                        var embed = EmbedUtils.GetEmbed("Audios not found", audiosNotFound);

                        await commandContext.RespondAsync(null, false, embed);
                    }

                    if (filePaths.Count > 0)
                    {
                        var voiceNext = commandContext.Client.GetVoiceNext();
                        var channel = commandContext.Member?.VoiceState?.Channel;

                        if (channel != null)
                        {
                            var voiceNextConnection = await voiceNext.ConnectAsync(channel);

                            await _audioService.SendAsync(voiceNextConnection, filePaths);

                            voiceNextConnection.Disconnect();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await commandContext.RespondAsync(e.ToString());
            }
        }

        [Command("playlist")]
        public async Task Playlist(CommandContext commandContext)
        {
            try
            {
                var audiosFolder = _configuration["audiosfolder"];
                var audioNames = Directory.EnumerateFiles(audiosFolder, "*.mp3", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileNameWithoutExtension)
                    .OrderBy(e => e, StringComparer.OrdinalIgnoreCase);
                var embed = EmbedUtils.GetEmbed("Playlist", audioNames);

                await commandContext.RespondAsync(null, false, embed);
            }
            catch (Exception e)
            {
                await commandContext.RespondAsync(e.ToString());
            }
        }

        [Command("repeat")]
        public async Task Repeat(CommandContext commandContext, string audioName, int repeat)
        {
            try
            {
                var filePaths = GetFilePaths(audioName.Yield(), out var audiosNotFound);

                if (audiosNotFound.Count > 0)
                {
                    var embed = EmbedUtils.GetEmbed("Audios not found", audiosNotFound);

                    await commandContext.RespondAsync(null, false, embed);
                }

                if (filePaths.Count > 0 && repeat > 0)
                {
                    var voiceNext = commandContext.Client.GetVoiceNext();
                    var channel = commandContext.Member?.VoiceState?.Channel;

                    if (channel != null)
                    {
                        var voiceNextConnection = await voiceNext.ConnectAsync(channel);

                        for (var i = 0; i < repeat; i++)
                            await _audioService.SendAsync(voiceNextConnection, filePaths);

                        voiceNextConnection.Disconnect();
                    }
                }
            }
            catch (Exception e)
            {
                await commandContext.RespondAsync(e.ToString());
            }
        }
    }
}
