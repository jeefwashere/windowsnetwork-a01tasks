//
// FILE               : ValdiationClass.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is where the validation for the app config settings are done
//
//
// Name               : ValdiationClass.cs            
// Purpose            : The class ip, port, and any file names are validated
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
        const int KInvalidPort = -1;
        /// <summary>
        /// A method to validate IP
        /// </summary>
        /// <param name="ip">Ip to be validated</param>
        /// <returns>The valid Ip address</returns>
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

            if(!IsValidAddress)
            {
                parsed = null;
            }
            return parsed;
        }
        /// <summary>
        /// A method to valid the port number
        /// </summary>
        /// <returns>The validated port</returns>
        public int ValudatePort()
        {
            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings["Port"], out port))
            {
                port = KInvalidPort;
                
            }
            if(port < 0 || port  > 65535)
            {
                port = KInvalidPort;
            }


            return port;
        }
        /// <summary>
        /// A method to validat the a file name
        /// </summary>
        /// <param name="fileName">the file naem to be validated</param>
        /// <returns>The validated name</returns>
        public string CheckFileName(string fileName)
        {
            
            if (string.IsNullOrEmpty(fileName)) {
                fileName = "";
            }
             return fileName;

        }
    }
}
