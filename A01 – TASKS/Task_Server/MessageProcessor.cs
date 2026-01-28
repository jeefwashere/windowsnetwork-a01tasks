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

namespace A01___TASKS
{
    internal class MessageProcessor
    {
        public async Task<bool> WriteMessageToFile(string message, string filePath, string logName, int maxFileSize, CancellationToken cts)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            bool sizeReachedFlag = false;

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = "output.txt";
            }

            if (string.IsNullOrEmpty(logName))
            {
                logName = "logger.log";
            }

            while (!sizeReachedFlag)
            {
                if (fileInfo.Length >= maxFileSize)
                {
                    sizeReachedFlag = true;
                }

                await File.WriteAllTextAsync(fileInfo.FullName, message, cts);
                // logger
            }

            return sizeReachedFlag;
        }
    }
}
