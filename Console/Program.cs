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
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            var serviceProvider = new ServiceCollection()
                .AddTransient<IDiscordProvider, DiscordProvider>()
                .AddTransient<IAudioService, AudioService>()
                .AddTransient<IConfiguration>(e => configurationBuilder.Build())
                .BuildServiceProvider();

            var discordProvider = serviceProvider.GetService<IDiscordProvider>();
            await discordProvider.Initialize();
        }
    }
}
