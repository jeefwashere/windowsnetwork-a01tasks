//
// FILE               : Server.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is where the server establish a connection to the client
// 
// Name               : Server            
// Purpose            : The server will establish a connection to the client using the TCP/IP reading the 
//                      IP and port from a config file.
using System;
using System.Net;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    internal class Server
    {
        int KInvalidPort = -1;
        IPAddress validIP;
        int validPort;
        string validServerFileName;
        string validLoggerName;
        ValidationClass validator =  new ValidationClass();

        public void Run()
        {
            GetConfigInfo();


            //Establish connection

            //Reference MessageProcessr.cs
        }

        public void GetConfigInfo()
        {
            string address = ConfigurationManager.AppSettings["IPAddress"] ?? "127.0.0.1";
            Console.WriteLine(address); //Check works
            validIP = validator.ValidateIp(address); //happy path works 
            validPort = validator.ValudatePort();
            if(validPort == KInvalidPort)
            {
                //logger invalidport
            }
            validServerFileName = ConfigurationManager.AppSettings["ServerFileName"] ?? string.Empty;
            validLoggerName = ConfigurationManager.AppSettings["LoggerFileName"] ?? string.Empty;
            Console.WriteLine(address); //Check works
            Console.WriteLine(validPort.ToString());
            Console.WriteLine(validServerFileName);
            Console.WriteLine(validLoggerName);
            //Check works
        }



        public void ConnectedToClient()
        {

        }
    }
}
