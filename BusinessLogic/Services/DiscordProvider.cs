using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BusinessLogic.Commands;
using BusinessLogic.Services.Interfaces;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace BusinessLogic.Services
{
    public class DiscordProvider : IDiscordProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DiscordProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Initialize()
        {
            var token = await ReadToken();
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

            await discordClient.ConnectAsync();

            await Task.Delay(-1);
        }

        private static async Task<string> ReadToken()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var folder = Path.GetDirectoryName(assemblyLocation);
            var tokenFile = Path.Combine(folder, "Token.txt");
            var token = await File.ReadAllTextAsync(tokenFile);

            return token;
        }
    }
}
