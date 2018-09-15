using System.Threading.Tasks;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IDiscordProvider, DiscordProvider>()
                .AddSingleton<IAudioService, AudioService>()
                .BuildServiceProvider();

            var discordProvider = serviceProvider.GetService<IDiscordProvider>();
            await discordProvider.Initialize();
        }
    }
}
