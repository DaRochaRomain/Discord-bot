using System;
using System.Threading.Tasks;
using BusinessLogic.Commands;
using BusinessLogic.Services.Interfaces;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Services
{
    public class DiscordProvider : IDiscordProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public DiscordProvider(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task Initialize()
        {
            var token = _configuration["DiscordToken"];
            var discordConfiguration = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            };
            var commandsNextConfiguration = new CommandsNextConfiguration
            {
                CaseSensitive = false,
                Services = _serviceProvider,
                StringPrefixes = new[] {"!sbb"}
            };

            var discordClient = new DiscordClient(discordConfiguration);
            var commands = discordClient.UseCommandsNext(commandsNextConfiguration);
            discordClient.UseVoiceNext();

            commands.RegisterCommands<DebugModule>();
            commands.RegisterCommands<GeneralModule>();
            commands.RegisterCommands<AudioModule>();
            commands.RegisterCommands<GifModule>();

            await discordClient.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}
