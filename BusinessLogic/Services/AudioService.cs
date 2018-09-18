using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using DSharpPlus.VoiceNext;

namespace BusinessLogic.Services
{
    public class AudioService : IAudioService
    {
        private static Process CreateProcess(string filePath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var process = Process.Start(processStartInfo);

            return process;
        }

        public async Task SendAsync(VoiceNextConnection connection, List<string> filePaths)
        {
            try
            {
                foreach (var filePath in filePaths)
                {
                    using (var process = CreateProcess(filePath))
                    {
                        var baseStream = process.StandardOutput.BaseStream;
                        var buffer = new byte[3840];
                        int bytesRead;

                        while ((bytesRead = baseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (bytesRead < buffer.Length)
                            {
                                for (var i = bytesRead; i < buffer.Length; i++)
                                    buffer[i] = 0;
                            }

                            await connection.SendAsync(buffer, 20);
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                //Ignored
            }
        }
    }
}
