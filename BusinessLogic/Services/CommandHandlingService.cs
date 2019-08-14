using System;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Services
{
    public class CommandHandlingService
    {
        private const string CommandPrefix = "!sbb ";
        private readonly CommandService _commandService;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IDiscordSocketClientService _discordSocketClientService;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _discordSocketClient = serviceProvider.GetService<DiscordSocketClient>();
            _commandService = serviceProvider.GetService<CommandService>();
            _discordSocketClientService = serviceProvider.GetService<IDiscordSocketClientService>();
        }

        public void Initialize()
        {
            _discordSocketClient.MessageReceived += DiscordSocketClientOnMessageReceived;
            _discordSocketClient.UserVoiceStateUpdated += _discordSocketClientService.DiscordSocketClientOnUserVoiceStateUpdated;
        }

        private async Task DiscordSocketClientOnMessageReceived(SocketMessage socketMessage)
        {
            var argPos = 0;

            if (socketMessage.Source == MessageSource.User && socketMessage is SocketUserMessage socketUserMessage)
            {
                var hasPrefix = socketUserMessage.HasStringPrefix(CommandPrefix, ref argPos, StringComparison.OrdinalIgnoreCase);

                if (hasPrefix)
                {
                    var socketCommandContext = new SocketCommandContext(_discordSocketClient, socketUserMessage);

                    argPos = CommandPrefix.Length;
                    await _commandService.ExecuteAsync(socketCommandContext, argPos, _serviceProvider);
                }
            }
        }
    }
}