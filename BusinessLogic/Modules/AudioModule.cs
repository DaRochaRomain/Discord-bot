using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BusinessLogic.Modules.Interfaces;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hangfire;
using Microsoft.Extensions.Configuration;
using NaturalSort.Extension;

namespace BusinessLogic.Modules
{
    public class AudioModule : ModuleBase<SocketCommandContext>, IAudioModule
    {
        private static readonly SemaphoreSlim PlaySemaphore = new SemaphoreSlim(1);
        private static ConcurrentBag<string> PlayJobs = new ConcurrentBag<string>();
        private readonly IAudioService _audioService;
        private readonly IConfiguration _configuration;
        private readonly DiscordSocketClient _discordSocketClient;

        public AudioModule(
            IAudioService audioService,
            IConfiguration configuration,
            DiscordSocketClient discordSocketClient)
        {
            _audioService = audioService;
            _configuration = configuration;
            _discordSocketClient = discordSocketClient;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play(params string[] titles)
        {
            try
            {
                if (Context.User is IGuildUser guildUser)
                {
                    var audiosFolder = _configuration["AudiosFolder"];
                    var files = titles
                        .Select(title => Path.Combine(audiosFolder, $"{title}.mp3"))
                        .Where(File.Exists)
                        .ToArray();

                    if (guildUser.VoiceChannel != null)
                    {
                        var jobId = BackgroundJob.Enqueue<IAudioModule>(audioModule => audioModule.PlayFiles(guildUser.VoiceChannel.Id, files));
                        PlayJobs.Add(jobId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task PlayFiles(ulong guidId, string[] files)
        {
            await PlaySemaphore.WaitAsync();

            try
            {
                var guild = (IVoiceChannel) _discordSocketClient.GetChannel(guidId);

                await _audioService.PlayFiles(guild, files);
            }
            finally
            {
                PlaySemaphore.Release();
            }
        }

        [Command("playsearch", RunMode = RunMode.Async)]
        public async Task PlaySearch(params string[] titles)
        {
            try
            {
                if (Context.User is IGuildUser guildUser)
                {
                    var audiosFolder = _configuration["AudiosFolder"];
                    var files = titles
                        .SelectMany(title => Directory.EnumerateFiles(audiosFolder, title))
                        .ToArray();

                    if (guildUser.VoiceChannel != null)
                        await _audioService.PlayFiles(guildUser.VoiceChannel, files);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        [Command("playlist", RunMode = RunMode.Async)]
        public async Task Playlist()
        {
            try
            {
                var audiosFolder = _configuration["AudiosFolder"];
                var fileList = Directory.EnumerateFiles(audiosFolder)
                    .Select(Path.GetFileNameWithoutExtension)
                    .OrderBy(fileName => fileName, StringComparer.OrdinalIgnoreCase.WithNaturalSort())
                    .Aggregate((fileName1, fileName2) => $"{fileName1}{Environment.NewLine}{fileName2}");

                await ReplyAsync(fileList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("playrandom", RunMode = RunMode.Async)]
        public async Task PlayRandom()
        {
            try
            {
                if (Context.User is IGuildUser guildUser)
                {
                    var audiosFolder = _configuration["AudiosFolder"];
                    var fileList = Directory.EnumerateFiles(audiosFolder)
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                    var random = new Random();
                    var randomValue = random.Next(fileList.Length);
                    var file = fileList[randomValue];


                    if (guildUser.VoiceChannel != null)
                        await _audioService.PlayFiles(guildUser.VoiceChannel, file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}