using System.Threading.Tasks;
using Discord.Audio;

namespace BusinessLogic.Services.Interfaces
{
    public interface IAudioService
    {
        Task SendAsync(IAudioClient client, string path);
    }
}