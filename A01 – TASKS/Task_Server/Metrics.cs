using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    internal class Metrics
    {
        // Wrote this function to measure time
        public async Task MeasureFileWriteTime(int clientCount, int messageSize, int bufferSize, Stopwatch totalWriteTime, string filePath, long maxFileSize, string logFilePath)
        {
            bool sizeReached = false;
            totalWriteTime.Restart();

            if (string.IsNullOrEmpty(logFilePath))
            {
                logFilePath = "metrics.log";
            }

            while (!sizeReached)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists && fileInfo.Length >= maxFileSize)
                {
                    sizeReached = true;
                }

                await Task.Delay(10);
            }

            totalWriteTime.Stop();
            await Logger.WriteLoggerAsync(
                @$"[TIME TO WRITE TO MAX FILE SIZE]
                Clients: {clientCount}
                TimeMs: {totalWriteTime.ElapsedMilliseconds}
                Max File Size: {maxFileSize}
                Message Size: {messageSize}
                Buffer Size: {bufferSize}", logFilePath);
        }
    }
}
