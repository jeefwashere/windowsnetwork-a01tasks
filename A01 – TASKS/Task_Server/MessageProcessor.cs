//
// FILE               : MessageProcessor.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Processes messages and writes to file safely (thread-safe using lock).
//
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Task_Server;

namespace A01___TASKS
{
    internal class MessageProcessor
    {
        private static readonly object fileLocker = new object();

        public Task<bool> CheckFile(string message, string filePath, string logFileName, double maxFileSize)
        {
            FileIO fileIO = new FileIO();
            bool fileSizeReached = false;

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = "output.txt";
            }

            if (string.IsNullOrEmpty(logFileName))
            {
                logFileName = "logger.log";
            }

            lock (fileLocker)
            {
                if (!File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(
                        filePath,
                        FileMode.OpenOrCreate,
                        FileAccess.Write,
                        FileShare.ReadWrite))
                    {
                    }
                }

                FileInfo fileInfo = new FileInfo(filePath);

                long messageBytes = Encoding.UTF8.GetByteCount(message);
                long projectedSize = fileInfo.Length + messageBytes;

                if (maxFileSize > 0 && projectedSize > (long)maxFileSize)
                {
                    fileSizeReached = true;
                }
                else
                {
                    fileIO.WriteToFileAsync(fileInfo.FullName, message).GetAwaiter().GetResult();
                    Logger.WriteLoggerAsync("Message Received: " + message, logFileName).GetAwaiter().GetResult();
                }
            }

            return Task.FromResult(fileSizeReached);
        }
    }
}
