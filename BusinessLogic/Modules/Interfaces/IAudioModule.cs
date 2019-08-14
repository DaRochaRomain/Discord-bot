using System.Threading.Tasks;

namespace BusinessLogic.Modules.Interfaces
{
    public interface IAudioModule
    {
        Task PlayFiles(ulong guidId, string[] files);
    }
}