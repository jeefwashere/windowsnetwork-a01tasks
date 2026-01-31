//
// FILE               : ClientConnector.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER         : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Client establishes a TCP connection to the server, sends FILESIZE,
//                      then repeatedly sends DATA until server replies FULL.
//                      One client runs per program instance.
//
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace A01___TASKS
{
    internal class ClientConnector
    {
         

        public async Task RunAsync()
        {
            int messageLength = int.Parse(ConfigurationManager.AppSettings["MessageLength"] ?? "12");
            string ipAddress = ConfigurationManager.AppSettings["Ipaddress"] ?? "127.0.0.1";
            string port = ConfigurationManager.AppSettings["Port"] ?? "14000";
            string sizeDoc = ConfigurationManager.AppSettings["size"] ?? "0";
            string cilentNumberString=ConfigurationManager.AppSettings["ClientCount"] ?? "50";
            int cilentNumber = int.Parse(cilentNumberString);
            //   One client per instance
            

            try
            {
                CancellationTokenSource token = new CancellationTokenSource();
                Task[] tasks = new Task[cilentNumber];

                for (int i = 0; i < cilentNumber; i++)
                {
                    int clientId = i + 1; // define each client
                    tasks[i] = RunSingleClientAsync(clientId, ipAddress, port, sizeDoc, messageLength, token);
                }

                Task.WaitAll(tasks);


            }
            catch (AggregateException aggEx)
            {
                Console.WriteLine("AggregateException caught. Inner exceptions:\n");

                aggEx = aggEx.Flatten();

                foreach (Exception ex in aggEx.InnerExceptions)
                {
                    
                    if (ex is SocketException)
                    {
                        Console.WriteLine(" SocketException: " + ex.Message);
                    }
                    else if (ex is IOException)
                    {
                        Console.WriteLine("  IOException: " + ex.Message);
                    }
                    else if (ex is OperationCanceledException)
                    {
                        Console.WriteLine(" OperationCanceledException: " + ex.Message);
                    }
                    else
                    {
                        Console.WriteLine(" " + ex.GetType().Name + ": " + ex.Message);
                    }
                }
            }

        }

        private async Task RunSingleClientAsync(int clientId, string ipAddress, string port, string sizeDoc, int messageLength, CancellationTokenSource token)
        {
            MessageSender sender = new MessageSender();
            TcpClient client = null;

            try
            {
                client = await ConnectToServerAsync(ipAddress, port);

                if (client == null)
                {
                    Console.WriteLine("[Client " + clientId + "] Failed to connect.");
                    return;
                }

                // 1) Send FILESIZE
                string fileSizeMsg = "FILESIZE " + sizeDoc;
                await sender.SendAsync(client, fileSizeMsg);
                string ack1 = await sender.ReceiveAsync(client);
                //do we handle ack make sure?
                Console.WriteLine("[Client " + clientId + "] Sent: " + fileSizeMsg + " | Server: " + ack1);

                // 2) Keep sending DATA until FULL
                
                int sentCount = 0;

                while (!token.IsCancellationRequested)
                {
                    string dataMsg = RandomString(messageLength);

                    await sender.SendAsync(client, dataMsg);
                    string response = await sender.ReceiveAsync(client);

                    sentCount++;

                    if (response == "FULL")
                    {
                        Console.WriteLine("[Client " + clientId + "] Server says FULL after " + sentCount + " messages. Stopping.");
                        token.Cancel();
                    }
                    else
                    {
                        Console.WriteLine("[Client " + clientId + "] Sent #" + sentCount + " (" + dataMsg.Length + " bytes) | Server: " + response);
                    }
                }

                // 3) Optional STOP (polite disconnect)
                await sender.SendAsync(client, "STOP");
                string ackStop = await sender.ReceiveAsync(client);
                Console.WriteLine("[Client " + clientId + "] Sent STOP | Server: " + ackStop);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Client " + clientId + "] Error: " + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        private async Task<TcpClient> ConnectToServerAsync(string ipAddress, string port)
        {
            TcpClient client = null;

            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);
                int portInt = int.Parse(port);

                client = new TcpClient();
                await client.ConnectAsync(ip, portInt);
            }
            catch
            {
                client = null;
            }

            return client;
        }

        private string RandomString(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be >= 0");
            }

            return new string('c', length);
        }
    }
}
