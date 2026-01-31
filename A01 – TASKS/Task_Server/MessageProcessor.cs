//
// FILE               : MessageProcessor.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file where the messages are processed using the task aysncrhonously
// 
// Name               : MessageProcessor.cs            
// Purpose            : Task are created to write to the file asynchronously
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Task_Server;

namespace A01___TASKS
{
    internal class MessageProcessor
    {
        private static readonly object fileLocker = new object();
        public async Task<bool> CheckFile(string message, string filePath, string logFileName, double maxFileSize)
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
                // Ensure file exists (prevents FileNotFoundException)
                if (!File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(
                        filePath,
                        FileMode.OpenOrCreate,
                        FileAccess.Write,
                        FileShare.ReadWrite))
                    {
                        // create then close
                    }
                }

                FileInfo fileInfo = new FileInfo(filePath);

                if (maxFileSize > 0 && fileInfo.Length >= maxFileSize)
                {
                    fileSizeReached = true;
                }
                else
                {
                    
                    fileIO.WriteToFileAsync(fileInfo.FullName, message).GetAwaiter().GetResult();
                    Logger.WriteLoggerAsync($"Message Received: {message}", logFileName).GetAwaiter().GetResult();
                }
            } 
            return fileSizeReached;
        }

    }
}
