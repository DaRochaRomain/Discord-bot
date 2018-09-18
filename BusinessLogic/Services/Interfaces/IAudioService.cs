using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;

namespace BusinessLogic.Services.Interfaces
{
    public interface IAudioService
    {
        Task SendAsync(VoiceNextConnection connection, List<string> filePaths);
    }
}
