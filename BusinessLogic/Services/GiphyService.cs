using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;

namespace BusinessLogic.Services
{
    public class GiphyService : IGiphyService
    {
        private readonly Giphy _giphy;

        public GiphyService(Giphy giphy)
        {
            _giphy = giphy;
        }

        public async Task<string> GetRandomGif(string tag)
        {
            try
            {
                var randomParameter = new RandomParameter
                {
                    Tag = tag
                };
                var gifresult = await _giphy.RandomGif(randomParameter);
                var url = gifresult.Data.Url;

                return url;
            }
            catch
            {
                return null;
            }
        }
    }
}