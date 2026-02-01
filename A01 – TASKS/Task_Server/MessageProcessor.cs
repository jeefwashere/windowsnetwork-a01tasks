//
// FILE               : MessageProcessor.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Processes messages and writes to file safely (thread-safe using lock).
//
// Name               : MessageProcessor.cs            
// Purpose            : Processor message and using aysnc tasks to write to a file 

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Task_Server;

namespace A01___TASKS
{
    internal class MessageProcessor
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1); // Found this on: https://stackoverflow.com/questions/20084695/lock-and-async-method-in-c-sharp
        /// <summary>
        /// A method to write to check file and write to file
        /// </summary>
        /// <param name="message">Message to be written</param>
        /// <param name="filePath">File path to be written to</param>
        /// <param name="logFileName">Log file name</param>
        /// <param name="maxFileSize">max file size for server</param>
        /// <returns>A task that represents writing async to a file</returns>
        public async Task<bool> CheckFile(string message, string filePath, string logFileName, long maxFileSize)
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

            await semaphoreSlim.WaitAsync(); // Explored further in the referenced link in the stackoverflow thread: https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
            try
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

                if(File.Exists(filePath) &&  fileInfo.Length >= maxFileSize )
                {
                    await File.WriteAllTextAsync(fileInfo.FullName, string.Empty);
                }

                long messageBytes = Encoding.UTF8.GetByteCount(message);
                long projectedSize = fileInfo.Length + messageBytes;

                if (maxFileSize > 0 && projectedSize > (long)maxFileSize)
                {
                    fileSizeReached = true;
                }
                else
                {
                    await fileIO.WriteToFileAsync(fileInfo.FullName, message);
                    await Logger.WriteLoggerAsync("Message Received: " + message, logFileName);
                }
            }
            catch (Exception ex)
            {
                await Logger.WriteLoggerAsync("Exceotuib Received: " + ex, logFileName);
            }
            finally
            {
                semaphoreSlim.Release();
            }

            return fileSizeReached;
        }
    }
}
