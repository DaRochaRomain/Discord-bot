using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface IGiphyService
    {
        Task<string> GetRandomGif(string tag);
        void Initialize();
    }
}
