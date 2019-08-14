using System.Diagnostics;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Discord;
using Discord.Audio;

namespace BusinessLogic.Services
{
    public class AudioService : IAudioService
    {
        public async Task PlayFiles(IVoiceChannel voiceChannel, params string[] files)
        {
            using (var audioClient = await voiceChannel.ConnectAsync())
            {
                foreach (var file in files)
                    await SendAsync(audioClient, file);
            }
        }

        private static async Task SendAsync(IAudioClient client, string path)
        {
            using (var process = CreateProcess(path))
            {
                using (var output = process.StandardOutput.BaseStream)
                {
                    using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
                    {
                        try
                        {
                            await output.CopyToAsync(discord);
                        }
                        finally
                        {
                            await discord.FlushAsync();
                        }
                    }
                }
            }
        }

        private static Process CreateProcess(string filePath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{filePath}\" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var process = Process.Start(processStartInfo);

            return process;
        }
    }
}