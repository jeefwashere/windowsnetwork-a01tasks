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
    }
}
