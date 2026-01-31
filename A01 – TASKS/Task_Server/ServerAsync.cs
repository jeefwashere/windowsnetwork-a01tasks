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
        string validServerFileName = "";
        string validLoggerName = "";
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
                    TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
                    Console.WriteLine("[SERVER] Client connected."); 
                    _ = Task.Run(() => ProcessRequest(client, cancellationToken), cancellationToken); // referenced this maybe
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("[SERVER] Socket Exception: " + ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[SERVER] Accept error: " + ex);
                }
            }
        }

        public async Task ProcessRequest(TcpClient client, CancellationToken cancellationToken)
        {
            double fileSize = 0.0;
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[4096];

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int count = await stream.ReadAsync(data, 0, data.Length, cancellationToken);

                    // client disconnected
                    if (count == 0)
                        break;

                    string incomingData = Encoding.UTF8.GetString(data, 0, count);

                    // optional stop command (ends this client connection)
                    if (incomingData.StartsWith("STOP", StringComparison.OrdinalIgnoreCase))
                    {
                        await SendAckAsync(stream, cancellationToken);
                        break;
                    }
                    // Handle STOP command
                    if (incomingData.StartsWith("STOP", StringComparison.OrdinalIgnoreCase))
                    {
                        await SendAckAsync(stream, cancellationToken);
                        break;
                    }

                    // ✅ SAFE FILESIZE parsing (PUT THIS HERE)
                    if (incomingData.StartsWith("FILESIZE", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] parts = incomingData.Split(
                            new char[] { ' ', ':', '=' },
                            StringSplitOptions.RemoveEmptyEntries
                        );

                        if (parts.Length >= 2)
                        {
                            double parsedSize;
                            bool ok = double.TryParse(parts[1], out parsedSize);

                            if (ok)
                            {
                                fileSize = parsedSize;
                                Console.WriteLine("[SERVER] Parsed fileSize = " + fileSize);
                            }
                            else
                            {
                                Console.WriteLine("[SERVER] FILESIZE parse failed: " + incomingData);
                            }
                        }
                        else
                        {
                            Console.WriteLine("[SERVER] FILESIZE format invalid: " + incomingData);
                        }

                        await SendAckAsync(stream, cancellationToken);
                        continue;   
                    }

                    // Handle normal data
                    await processor.CheckFile(incomingData, validServerFileName, validLoggerName, fileSize);
                    Console.WriteLine("I got this: " + incomingData);

                    await SendAckAsync(stream, cancellationToken);


                }
            }
            catch (OperationCanceledException)
            {
                // expected on shutdown
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SERVER] Error " + ex);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

        private static async Task SendAckAsync(NetworkStream stream, CancellationToken token)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes("ack");
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length, token);
        }

    }
}
