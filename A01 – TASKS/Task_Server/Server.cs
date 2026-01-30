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
using System.Net.Sockets;
using A01___TASKS;

namespace Task_Server
{
    internal class Server
    {
        int KInvalidPort = -1;
        IPAddress validIP;
        int validPort;
        string validServerFileName;
        string validLoggerName;
        double fileSize;
        ValidationClass validator =  new ValidationClass();
        Parser parser = new Parser();
        MessageProcessor processor = new MessageProcessor();

        private volatile bool run = true;
        public void Run()
        {
            GetConfigInfo();

            ConnectedToClient();
            //Establish connection

            //Reference MessageProcessr.cs
        }

        public void GetConfigInfo()
        {
            string address = ConfigurationManager.AppSettings["IPAddress"] ?? "127.0.0.1";
            validIP = validator.ValidateIp(address); //happy path works 
            validPort = validator.ValudatePort();
            if(validPort == KInvalidPort)
            {
                //logger invalidport
            }
            validServerFileName = ConfigurationManager.AppSettings["ServerFileName"] ?? string.Empty;
            validLoggerName = ConfigurationManager.AppSettings["LoggerFileName"] ?? string.Empty;

            Console.WriteLine($"[SERVER CONFIG] IP={validIP} PORT={validPort}");
        }



        public void ConnectedToClient()
        {
            TcpListener listener = new TcpListener(validIP, validPort);
            listener.Start();
            while (run) {
                try
                {
                    //Create a listener client
                    TcpClient client = listener.AcceptTcpClient();
                    ParameterizedThreadStart paramThread = new ParameterizedThreadStart(ProcessRequest);
                    Thread clientThread = new Thread(paramThread);
                    clientThread.Start(client);
                }
                catch (SocketException ex)
                {
                    //log
                }
                catch (Exception ex)
                {
                    //log
                }
            }
        }

        private void ProcessRequest(Object o)
        {
            // Process client request
            TcpClient client = (TcpClient)o;
            NetworkStream stream = client.GetStream();

            // Buffer for reading data
            byte[] data = new byte[1024];
            string incomingData = "";
            string response = "";

            int count;

            try
            {
                // Read the client request (only one request per connection)
                count = stream.Read(data, 0, data.Length);
                Console.Write("I got this: " + count);
                if (count > 0)
                {
                    incomingData = Encoding.ASCII.GetString(data, 0, count);
                    //Logger.Log("Received data: " + incomingData);
                    if(incomingData.StartsWith("FILESIZE"))
                    {
                         fileSize = parser.ParseFileSizeMessage(incomingData);
                    } else
                    {
                        Console.Write("I got this: " + incomingData);
                        // processor.CheckFile(incomingData, validServerFileName, validLoggerName, fileSize);
                    }
                    response = "ack";
                    if (response.Length > 0)
                    {
                        byte[] respBytes = Encoding.ASCII.GetBytes(response);
                        stream.Write(respBytes, 0, respBytes.Length);

                        //log response
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Log($"Error in ProcessRequest: {ex}");
            }
            finally
            {
                //Clean Up
                if (stream != null)
                {
                    stream.Close();
                }

                if (client != null)
                {
                    //Logger.Log("Client connection closed.");
                    client.Close();
                }
            }
        }
        


    }
}
