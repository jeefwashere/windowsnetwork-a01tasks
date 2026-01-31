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
        public static async Task WriteLoggerAsync(string logMessage, string logFilePath)
        {
            FileIO fileIo = new FileIO();

            if (string.IsNullOrEmpty(logFilePath))
            {
                logFilePath = "logger.log";
            }

            string fullMessage = $"[{DateTime.UtcNow}] | {logMessage}";

            await fileIo.WriteToFileAsync(logFilePath, fullMessage);
        }
    }
}