//
// FILE               : Logger.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is ]where logs are made
// 
// Name               : Logger.cs            
// Purpose            : logs any messages recived, measurements and error messages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    public class Logger
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1); // Found this on: https://stackoverflow.com/questions/20084695/lock-and-async-method-in-c-sharp
        /// <summary>
        /// A method to log any server information
        /// </summary>
        /// <param name="logMessage">The message to be logged</param>
        /// <param name="logFilePath">The logfile name</param>
        /// <returns>Task that represent async for the logging</returns>
        public static async Task WriteLoggerAsync(string logMessage, string logFilePath, CancellationToken cancellationToken)
        {
            FileIO fileIo = new FileIO();

            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                string fullMessage =
                    $"[{DateTime.UtcNow}] | {logMessage}{Environment.NewLine}";

                // Let the write operation create the file if it doesn't exist
                await fileIo.WriteToFileAsync(
                    logFilePath,
                    fullMessage,
                    CancellationToken.None); // logging should not be cancelable
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}