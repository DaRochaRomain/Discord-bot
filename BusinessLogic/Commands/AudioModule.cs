using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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

        private async Task<List<string>> ParseAudioNames(CommandContext commandContext, params string[] audioNames)
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
                await commandContext.RespondAsync(errorStringBuilder.ToString());

            return filePaths;
        }

        [Command("play")]
        public async Task Play(CommandContext commandContext, params string[] audioNames)
        {
            try
            {
                if (audioNames != null)
                {
                    var filePaths = await ParseAudioNames(commandContext, audioNames);

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
                var playlist = _configuration
                    .AsEnumerable()
                    .Select(e => e.Key)
                    .OrderBy(e => e, StringComparer.OrdinalIgnoreCase)
                    .Aggregate((s1, s2) => $"{s1}{Environment.NewLine}{s2}");
                var embedBuilder = new DiscordEmbedBuilder();

                embedBuilder.AddField("Playlist", playlist);
                await commandContext.RespondAsync(string.Empty, false, embedBuilder.Build());
            }
            catch (Exception e)
            {
                await commandContext.RespondAsync(e.ToString());
            }
        }
    }
}
