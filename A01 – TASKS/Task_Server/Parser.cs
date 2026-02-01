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
        /// <param name="incomingData">Mesasge to be parsed</param>
        /// <returns>file size</returns>
        public long ParseFileSizeMessage(string incomingData)
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
                    Console.WriteLine("[SERVER] Parsed fileSize = " + fileSize);
                }
                else
                {
                    Console.WriteLine("[SERVER] FILESIZE parse failed: " + incomingData);
                    fileSize = -1;
                }
            }
            else
            {
                Console.WriteLine("[SERVER] FILESIZE format invalid: " + incomingData);
            }

            return fileSize;
        }

        public int ParseClientCount(string incomingData)
        {
            int clientCount = 0;
            string[] parts = incomingData.Split(
                            new char[] { ' ', ':', '=' },
                            StringSplitOptions.RemoveEmptyEntries
                        );


            if (parts.Length == 2)
            {
                if (int.TryParse(parts[1],out int parsedCount) && parsedCount > 0)
                {
                    clientCount = parsedCount;
                    Console.WriteLine("[SERVER] Parsed client count = " + parsedCount);
                }   
                else
                {
                    Console.WriteLine("[SERVER] Client count parse failed: " + parsedCount);
                    clientCount = -1;
                }
            }
            else
            {
                Console.WriteLine("[SERVER] Client count format invalid: " + incomingData);
            }

            return clientCount;
        }
    }
}
