using System;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord.Commands;

namespace BusinessLogic.Commands
{
    public class GifModule : ModuleBase<SocketCommandContext>
    {
        private readonly IGiphyService _giphyService;

        public GifModule(IGiphyService giphyService)
        {
            _giphyService = giphyService;
        }

        [Command("gif")]
        public async Task Gif(string tag)
        {
            try
            {
                var gif = await _giphyService.GetRandomGif(tag);

                if (gif != null)
                    await ReplyAsync(gif);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}