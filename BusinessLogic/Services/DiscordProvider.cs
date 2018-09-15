using System;
using System.Reflection;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BusinessLogic.Services
{
    public class DiscordProvider : IDiscordProvider
    {
        private readonly CommandService _commandService;
        private readonly DiscordSocketClient _discordClient;
        private readonly IServiceProvider _serviceProvider;

        public DiscordProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _discordClient = new DiscordSocketClient();
            _commandService = new CommandService();
        }

        private async Task DiscordClientOnMessageReceived(SocketMessage socketMessage)
        {
            if (socketMessage is IUserMessage userMessage)
            {
                var context = new CommandContext(_discordClient, userMessage);

                await _commandService.ExecuteAsync(context, 0, _serviceProvider);
            }
        }

        public async Task Initialize()
        {
            const string token = "NDkwMjk3ODE4ODEzMTY5Njc0.Dn3RLQ.QtLcmOFGbZZ6kXCTrsbGSknGSVw";

            await _discordClient.LoginAsync(TokenType.Bot, token);
            await _discordClient.StartAsync();

            _discordClient.MessageReceived += DiscordClientOnMessageReceived;
            await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly());

            await Task.Delay(-1);
        }
    }
}
