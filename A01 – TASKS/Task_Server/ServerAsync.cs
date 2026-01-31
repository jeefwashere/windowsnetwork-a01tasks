//
// FILE               : ServerAsync.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Server accepts clients and processes messages over TCP.
//                      Sends "ack" normally; sends "FULL" when output file limit reached.
//
using A01___TASKS;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            validIP = validator.ValidateIp(address);

            validPort = validator.ValudatePort();
            if (validPort == KInvalidPort)
            {
                // logger invalid port
            }

            validServerFileName = ConfigurationManager.AppSettings["ServerFileName"] ?? string.Empty;
            validLoggerName = ConfigurationManager.AppSettings["LoggerFileName"] ?? string.Empty;

            Console.WriteLine("[SERVER CONFIG] IP=" + validIP + " PORT=" + validPort);
        }

        public async Task GetClientConnectionAsync(CancellationToken cancellationToken)
        {
            TcpListener listener = new TcpListener(validIP, validPort);
            listener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
                    Console.WriteLine("[SERVER] Client connected.");

                    Console.WriteLine("Pre connection");
                    _ = Task.Run(() => ProcessRequest(client, cancellationToken), cancellationToken);
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
            long fileSize = 0;
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[4096];

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int count = await stream.ReadAsync(data, 0, data.Length, cancellationToken);

                    if (count == 0)
                    {
                        break; // disconnected
                    }

                    string incomingData = Encoding.UTF8.GetString(data, 0, count);

                    // STOP
                    if (incomingData.StartsWith("STOP", StringComparison.OrdinalIgnoreCase))
                    {
                        await SendResponseAsync(stream, "ack\n", cancellationToken);
                        break;
                    }

                    // FILESIZE
                    if (incomingData.StartsWith("FILESIZE", StringComparison.OrdinalIgnoreCase))
                    {
                        fileSize = parser.ParseFileSizeMessage(incomingData);
                        //if (fileSize <= 0)
                        //{
                        //    await SendResponseAsync(stream, "err", cancellationToken);
                        //    continue;
                        //} 
                        await SendResponseAsync(stream, "ack\n", cancellationToken);
                        continue;
                    }

                    // DATA
                    bool isFull = await processor.CheckFile(incomingData, validServerFileName, validLoggerName, fileSize);
                    Console.WriteLine("I got this: " + incomingData);

                    if (isFull)
                    {
                        await SendResponseAsync(stream, "FULL\n", cancellationToken);
                        break; // end connection when full
                    }
                    else
                    {
                        await SendResponseAsync(stream, "ack\n", cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // expected
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

        private static async Task SendResponseAsync(NetworkStream stream, string response, CancellationToken token)
        {
            Console.WriteLine("You're inside Response ACK");
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length, token);
        }
    }
}
