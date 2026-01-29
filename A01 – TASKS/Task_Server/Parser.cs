using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    internal class Parser
    {
        public double ParseFileSizeMessage(string message)
        {
            double response = 0.00;
            string[] parts = message.Split(":");
            bool valid = false;

            if (parts.Length != 2 && parts[0] != "FILESIZE")
            {
                response = -1;
                valid = true;
            }

            if (valid)
            {
                if (!double.TryParse(parts[1], out double fileSize))
                {
                    response = -1.00;
                    valid = false;
                }

                response = fileSize;
            }

            return response;
        }
    }
}
