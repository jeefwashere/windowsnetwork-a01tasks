using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    internal class ValidationClass
    {
        public bool IsValidAddress = true;
        int KInvalidPort = -1;
        public IPAddress? ValidateIp(string? ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                IsValidAddress = false;
            }

            ip = ip.Trim();

            if (!IPAddress.TryParse(ip, out IPAddress? parsed))
            {
                IsValidAddress = false;
            }

            // IPv4 only (remove this if IPv6 is allowed)
            if (parsed.AddressFamily != AddressFamily.InterNetwork)
            {
                IsValidAddress = false;
            }

            if(!IsValidAddress)
            {
                parsed = null;
            }
            return parsed;
        }
        public int ValudatePort()
        {
            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings["Port"], out port))
            {
                port = KInvalidPort;
                
            }
            if(port < 0 || port  > 656350)
            {
                port = KInvalidPort;
            }


            return port;
        }
        public string CheckFileName(string fileName)
        {
            
            if (string.IsNullOrEmpty(fileName)) {
                fileName = "";
            }
             return fileName;

        }
    }
}
