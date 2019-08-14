using System.Threading.Tasks;
using Discord;

namespace BusinessLogic.Services.Interfaces
{
    public interface IAudioService
    {
        Task PlayFiles(IVoiceChannel voiceChannel, params string[] files);
    }
}