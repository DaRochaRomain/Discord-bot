using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord.Audio;
using NAudio.Wave;

namespace BusinessLogic.Services
{
    public class AudioService : IAudioService
    {
        public async Task SendAsync(IAudioClient audioClient, params string[] filePaths)
        {
            try
            {
                var outFormat = new WaveFormat(48000, 16, 2);

                foreach (var filePath in filePaths)
                {
                    using (var stream = audioClient.CreatePCMStream(AudioApplication.Mixed))
                    {
                        using (var mp3Reader = new Mp3FileReader(filePath))
                        {
                            using (var resampler = new MediaFoundationResampler(mp3Reader, outFormat))
                            {
                                var buffer = new byte[4096];
                                resampler.ResamplerQuality = 60;
                                int byteCount;

                                while ((byteCount = resampler.Read(buffer, 0, buffer.Length)) > 0)
                                    await stream.WriteAsync(buffer, 0, byteCount);
                                await stream.FlushAsync();
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //Ignore
            }
        }
    }
}
