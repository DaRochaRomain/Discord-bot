using System.IO;
using System.Threading.Tasks;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IDiscordProvider, DiscordProvider>()
                .AddSingleton<IAudioService, AudioService>()
                .AddSingleton<IConfiguration>(configuration)
                .BuildServiceProvider();

            var discordProvider = serviceProvider.GetService<IDiscordProvider>();
            await discordProvider.Initialize();
        }
    }
}
