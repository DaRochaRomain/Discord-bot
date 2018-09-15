using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord.Audio;

namespace BusinessLogic.Services
{
    public class AudioService : IAudioService
    {
        private static Process CreateProcess(string path)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            return Process.Start(processStartInfo);
        }

        public async Task SendAsync(IAudioClient audioClient, string filePath)
        {
            try
            {
                using (var ffmpeg = CreateProcess(filePath))
                {
                    using (var stream = audioClient.CreatePCMStream(AudioApplication.Music))
                    {
                        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
                        await stream.FlushAsync();
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
