//
// FILE               : Parser.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is where messages are parsed for file size
// 
// Name               : Parser.cs            
// Purpose            : Parser messages based on the are messaing protocols to get filesize
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_Server
{
    internal class Parser
    {
        /// <summary>
        /// A method to parse the incoming data to get file sizxe
        /// </summary>
        /// <param name="incomingData">Message to be parsed</param>
        /// <returns>File size</returns>
        public async Task<long> ParseFileSizeMessage(string incomingData, string logFileName, CancellationToken cancellationToken)
        {
            long fileSize = 0;
            string[] parts = incomingData.Split(
                            new char[] { ' ', ':', '=' },
                            StringSplitOptions.RemoveEmptyEntries
                        );

            if (parts.Length == 2)
            {
                long parsedSize;

                if (long.TryParse(parts[1], out parsedSize))
                {
                    fileSize = parsedSize;
                    await Logger.WriteLoggerAsync("[SERVER] Parsed fileSize = " + fileSize, logFileName, cancellationToken);
                }
                else
                {
                    await Logger.WriteLoggerAsync("[SERVER] FILESIZE parse failed: " + incomingData, logFileName, cancellationToken);
                    fileSize = -1;
                }
            }
            else
            {
                await Logger.WriteLoggerAsync("[SERVER] FILESIZE format invalid: " + incomingData, logFileName, cancellationToken);
            }

            return fileSize;
        }
    }
}
