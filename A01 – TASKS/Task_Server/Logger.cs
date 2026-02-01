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
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    public class Logger
    {
        /// <summary>
        /// A method to log any server information
        /// </summary>
        /// <param name="logMessage">The message to be logged</param>
        /// <param name="logFilePath">The logfile name</param>
        /// <returns>Task that represent async for the logging</returns>
        public static async Task WriteLoggerAsync(string logMessage, string logFilePath, CancellationToken cancellationToken)
        {
            FileIO fileIo = new FileIO();

            if (!File.Exists(logFilePath))
            {
                using (FileStream fs = new FileStream(
                    logFilePath,
                    FileMode.OpenOrCreate,
                    FileAccess.Write,
                    FileShare.ReadWrite))
                {
                }
            }

            string fullMessage = $"[{DateTime.UtcNow}] | {logMessage}";

            await fileIo.WriteToFileAsync(logFilePath, fullMessage, cancellationToken);
        }
    }
}