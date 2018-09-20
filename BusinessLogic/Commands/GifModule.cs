using System;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BusinessLogic.Commands
{
    public class GifModule : BaseCommandModule
    {
        private readonly IGiphyService _giphyService;

        public GifModule(IGiphyService giphyService)
        {
            _giphyService = giphyService;
        }

        [Command("gif")]
        public async Task Gif(CommandContext commandContext, string tag)
        {
            try
            {
                var gif = await _giphyService.GetRandomGif(tag);

                await commandContext.RespondAsync(gif);
            }
            catch (Exception e)
            {
                await commandContext.RespondAsync(e.ToString());
            }
        }
    }
}
