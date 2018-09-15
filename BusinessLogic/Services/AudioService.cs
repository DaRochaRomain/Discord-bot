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
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            return Process.Start(ffmpeg);
        }

        public async Task SendAsync(IAudioClient client, string path)
        {
            using (var ffmpeg = CreateProcess(path))
            {
                var baseStream = ffmpeg.StandardOutput.BaseStream;

                using (var audioOutStream = client.CreatePCMStream(AudioApplication.Mixed))
                {
                    await baseStream.CopyToAsync(audioOutStream);
                    await audioOutStream.FlushAsync();
                }
            }
        }
    }
}
