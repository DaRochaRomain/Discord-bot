using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Services
{
    public class GiphyService : IGiphyService
    {
        private readonly IConfiguration _configuration;

        public GiphyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static Giphy Giphy { get; set; }

        public async Task<string> GetRandomGif(string tag)
        {
            var randomParameter = new RandomParameter
            {
                Tag = tag
            };

            var gifresult = await Giphy.RandomGif(randomParameter);
            var url = gifresult.Data.Url;

            return url;
        }

        public void Initialize()
        {
            var giphyToken = _configuration["GiphyToken"];

            Giphy = new Giphy(giphyToken);
        }
    }
}
