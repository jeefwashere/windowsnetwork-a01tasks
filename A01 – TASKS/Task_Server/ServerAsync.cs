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
using A01___TASKS;
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
    class ServerAsync
    {
        int KInvalidPort = -1;
        IPAddress validIP;
        int validPort;
        string validServerFileName;
        string validLoggerName;
        double fileSize;
        ValidationClass validator = new ValidationClass();
        Parser parser = new Parser();
        MessageProcessor processor = new MessageProcessor();

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            
            GetConfigInfo();
            await GetClientConnectionAsync(cancellationToken);
        }

        public void GetConfigInfo()
        {
            string address = ConfigurationManager.AppSettings["IPAddress"] ?? "127.0.0.1";
            validIP = validator.ValidateIp(address); //happy path works 

            if (validIP == null)
            {
                // log invalid IP address
                // Throw exception maybe?
            }

            validPort = validator.ValudatePort();
            if (validPort == KInvalidPort)
            {
                //logger invalidport
            }
            validServerFileName = ConfigurationManager.AppSettings["ServerFileName"] ?? string.Empty;
            validLoggerName = ConfigurationManager.AppSettings["LoggerFileName"] ?? string.Empty;

            Console.WriteLine($"[SERVER CONFIG] IP={validIP} PORT={validPort}");
        }

        public async Task GetClientConnectionAsync(CancellationToken cancellationToken)
        {
            TcpListener listener = new TcpListener(validIP, validPort);
            listener.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    //Create a listener client
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    await ProcessRequest(client); // Check if concurrency works
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

        public async Task ProcessRequest(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            // Data buffer
            byte[] data = new byte[1024];
            string incomingData = "";
            string response = "";
            int count = 0;

            try
            {
                count = stream.Read(data, 0, data.Length);
                if (count > 0)
                {
                    incomingData = Encoding.ASCII.GetString(data, 0, count);
                    //Logger.Log("Received data: " + incomingData);

                    if (incomingData.StartsWith("FILESIZE"))
                    {
                        fileSize = parser.ParseFileSizeMessage(incomingData);
                    }
                    else
                    {
                        await processor.CheckFile(incomingData, validServerFileName, validLoggerName, fileSize);

                    }
                    response = "ack";
                    if (response.Length > 0)
                    {
                        byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                        stream.Write(responseBytes, 0, responseBytes.Length);

                        //Logger.Log($"Response sent: {response}");
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Log($"Error in ProcessRequest: {ex}");
            }
            finally
            {
                // Cleanup
                if (stream != null)
                {
                    stream.Close();
                }

                if (client != null)
                {
                    client.Close();
                }
            }
        }
    }
}
