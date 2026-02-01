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
         
        /// <summary>
        /// this was how we run different cilent us ethe task
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            readConfig readConfig = new readConfig();
            int messageLength = readConfig.messageLength;
            string ipAddress = readConfig.ipAddress;
            string port = readConfig.port;
            string sizeDoc = readConfig.sizeDoc;
            int cilentNumber = readConfig.cilentNumber;
            //   One client per instance
            

            try
            {//create token to track
                CancellationTokenSource token = new CancellationTokenSource();
                Task[] tasks = new Task[cilentNumber];

                for (int i = 0; i < cilentNumber; i++)
                {
                    int clientId = i + 1; // define each client
                    tasks[i] = RunSingleClientAsync(clientId, ipAddress, port, sizeDoc, messageLength, token);
                }

                Task.WaitAll(tasks);// wait task finish


            }
            catch (AggregateException aggEx)
            {// error handle
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId">id </param>
        /// <param name="ipAddress">ip address</param>
        /// <param name="port">port</param>
        /// <param name="sizeDoc">size</param>
        /// <param name="messageLength">how long we send </param>
        /// <param name="token">calcel or not</param>
        /// <returns></returns>
        private async Task RunSingleClientAsync(int clientId, string ipAddress, string port, string sizeDoc, int messageLength, CancellationTokenSource token)
        {
            MessageSender sender = new MessageSender();
            TcpClient client = null;
            //this is create sender and tcp for client
            try
            {
                client = await ConnectToServerAsync(ipAddress, port);

                if (client == null)
                {// if error
                    Console.WriteLine("[Client is " + clientId + "] failed to connect.");
                    return;
                }

                // Send FILESIZE at first time let the server create one
                string fileSizeMsg = "FILESIZE " + sizeDoc;
                await sender.SendAsync(client, fileSizeMsg);
                string ack1 = await sender.ReceiveAsync(client);// take the ack to make sure server get
              // show we get
                Console.WriteLine("[Client " + clientId + "] Sent: " + fileSizeMsg + " | Server: " + ack1);

                // keep send data until server reply FULL

                int sentCount = 0;

                while (!token.IsCancellationRequested)
                {//get random
                    string dataMsg = RandomString(messageLength);
                    // send message
                    await sender.SendAsync(client, dataMsg);
                    string response = await sender.ReceiveAsync(client);

                    sentCount++;

                    if (response == "FULL")
                    {//if the server return back say full
                        Console.WriteLine("[Client " + clientId + "] Server says FULL after " + sentCount + " messages. Stopping.");
                        token.Cancel();
                    }
                    else
                    {
                        Console.WriteLine("[Client " + clientId + "] Sent #" + sentCount + " (" + dataMsg.Length + " bytes) | Server: " + response);
                    }
                }

                ////strop it 
                await sender.SendAsync(client, "STOP");
                string ackStop = await sender.ReceiveAsync(client);
                Console.WriteLine("[Client " + clientId + "] Sent STOP | Server: " + ackStop);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Client " + clientId + "] Error: " + ex.Message);
            }// handle error 
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }
        /// <summary>
        /// this is to connect to server
        /// </summary>
        /// <param name="ipAddress">ip</param>
        /// <param name="port">port</param>
        /// <returns></returns>
        private async Task<TcpClient> ConnectToServerAsync(string ipAddress, string port)
        {
            TcpClient client = null;

            try
            {// get all ipaddress and other corrent
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
        /// <summary>
        /// most important part to get random string
        /// </summary>
        /// <param name="length">easy wait</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private string RandomString(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be >= 0");
            }
            //rather than a for loop to += choice this more fance way
            //reference:https://learn.microsoft.com/en-us/dotnet/api/system.string.-ctor?view=net-10.0
            return new string('c', length);
        }
    }
}
