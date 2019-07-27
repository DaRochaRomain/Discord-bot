using System;
using System.IO;
using System.Threading.Tasks;
using BusinessLogic.Commands;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Console
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        public static async Task Main(string[] args)
        {
            ConfigureService();

            InitializeCommands();
            await InitializeDiscord();
        }

        private static void ConfigureService()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            var configuration = configurationBuilder.Build();
            var giphyToken = configuration["GiphyToken"];
            var giphy = new Giphy(giphyToken);

            _serviceProvider = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(giphy)
                .AddSingleton<IConfiguration>(e => configuration)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddTransient<IAudioService, AudioService>()
                .AddTransient<IGiphyService, GiphyService>()
                .BuildServiceProvider();
        }

        private static async Task InitializeDiscord()
        {
            var discordSocketClient = _serviceProvider.GetService<DiscordSocketClient>();
            var configuration = _serviceProvider.GetService<IConfiguration>();
            var token = configuration["DiscordToken"];

            discordSocketClient.Log += DiscordSocketClientOnLog;

            await discordSocketClient.LoginAsync(TokenType.Bot, token);
            await discordSocketClient.StartAsync();
            await Task.Delay(-1);
        }

        private static async Task DiscordSocketClientOnLog(LogMessage arg)
        {
            System.Console.WriteLine(arg.ToString());
        }

        private static void InitializeCommands()
        {
            var commandService = _serviceProvider.GetService<CommandService>();
            var commandHandlingService = _serviceProvider.GetService<CommandHandlingService>();

            commandService.AddModuleAsync<DebugModule>(_serviceProvider);
            commandService.AddModuleAsync<AudioModule>(_serviceProvider);
            commandService.AddModuleAsync<GifModule>(_serviceProvider);
            commandHandlingService.Initialize();
        }
    }
}