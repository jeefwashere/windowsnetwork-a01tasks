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

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length >= maxFileSize)
            {
                fileSizeReached = true;
            }
            else
            {
                await fileIO.WriteToFileAsync(fileInfo.FullName, message);
                await Logger.WriteLoggerAsync($"Message Received: {message}", logFileName);
            }

            return fileSizeReached;
        }
    }
}
