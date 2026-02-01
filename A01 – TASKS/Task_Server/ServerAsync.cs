//
// FILE               : ServerAsync.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Server accepts clients and processes messages over TCP.
//                      With set message protocols that depending how the message starts.
//                      Sends "ack" normally; sends "FULL" when output file limit reached.
//
// Name               : ServerAsync.cs            
// Purpose            : The class where the server logic is using async tasks
//  
using A01___TASKS;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_Server
{
    class ServerAsync
    {
        //Class Variables
        const int KInvalidPort = -1;
        IPAddress? validIP;
        int validPort;
        string validServerFileName = "";
        string validLoggerName = "";
        string validMetricsLoggerName = "";
        
        const int BUFFER_SIZE = 4096;
        int clientCount = 0;
        long maxFileSize = 0;
        int messageSize = 0;

        ValidationClass validator = new ValidationClass();
        Parser parser = new Parser();
        Metrics metrics;
        MessageProcessor processor;

        public ServerAsync()
        {
            metrics = new Metrics();
            processor = new MessageProcessor(metrics, BUFFER_SIZE);
        }

        /// <summary>
        /// A method to run the server
        /// </summary>
        /// <param name="cancellationToken">Cancellation token if need to shutdown task</param>
        /// <returns> Task that represent async start  the server</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await GetConfigInfo(cancellationToken); //Get Info from app config
            await ClientConnectionAsync(cancellationToken);
        }
        /// <summary>
        /// A method to get the config info and assing it to the class varibales
        /// </summary>
        /// <returns>Task that represent async getting appconfig</returns>
        public async Task GetConfigInfo(CancellationToken cancellationToken)
        {
            string address = ConfigurationManager.AppSettings["IPAddress"] ?? "127.0.0.1";
            validIP = validator.ValidateIp(address);

            validPort = validator.ValudatePort();
            if (validPort == KInvalidPort)
            {
                await Logger.WriteLoggerAsync("[SERVER CONFIG] Invalid port detected.", validLoggerName, cancellationToken);
            }

            validServerFileName = ConfigurationManager.AppSettings["ServerFileName"] ?? string.Empty;
            validLoggerName = ConfigurationManager.AppSettings["LoggerFileName"] ?? string.Empty;
            validMetricsLoggerName = ConfigurationManager.AppSettings["MetricsLoggerFileName"] ?? string.Empty;

            await Logger.WriteLoggerAsync("[SERVER CONFIG] IP=" + validIP + " PORT=" + validPort,validLoggerName, cancellationToken);
        }
        /// <summary>
        /// A method to get the connected to the clients and start processing the messages
        /// </summary>
        /// <param name="cancellationToken">Cancellation token if need to shutdown task</param>
        /// <returns>Task that represent async to get client connection</returns>
        public async Task ClientConnectionAsync(CancellationToken cancellationToken)
        {
            TcpListener listener = new TcpListener(validIP, validPort);
            listener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
                    Interlocked.Increment(ref clientCount); // Found how to increment a variable asynchronously safely here: https://stackoverflow.com/questions/32832770/increase-a-value-type-from-multiple-async-methods-i-e-threads-in-c-sharp
                    await Logger.WriteLoggerAsync("[SERVER] Client connected", validLoggerName, cancellationToken);
                    _ = ProcessRequest(client, cancellationToken); //Ignore return task
                }

                catch (SocketException ex)
                {
                    await Logger.WriteLoggerAsync("[SERVER] Socket Exception: " + ex, validLoggerName, cancellationToken);
                }
                catch (Exception ex)
                {
                    await Logger.WriteLoggerAsync("[SERVER] Accept error: " + ex, validLoggerName, cancellationToken);
                }
            }
        }
        /// <summary>
        /// A method to process the request looking for certain message protocols to
        /// process the messages
        /// </summary>
        /// <param name="client">Client that sent the message</param>
        /// <param name="cancellationToken">Cancellation token if need to shutdown tasks</param>
        /// <returns>Task that represent async to process request</returns>
        public async Task ProcessRequest(TcpClient client, CancellationToken cancellationToken)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[BUFFER_SIZE];

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    messageSize = await stream.ReadAsync(data, 0, data.Length, cancellationToken);

                    if (messageSize == 0) // client disconnected no clients
                    {
                        break; 
                    }

                    string incomingData = Encoding.UTF8.GetString(data, 0, messageSize);

                    
                    if (incomingData.StartsWith("STOP"))
                    {
                        await SendResponseAsync(stream, "ack\n", cancellationToken);
                        break;
                    }
  
                    if (incomingData.StartsWith("FILESIZE"))
                    {
                        maxFileSize = parser.ParseFileSizeMessage(incomingData);
                        await SendResponseAsync(stream, "ack\n", cancellationToken);
                        continue;
                    }

                    bool isFull = await processor.CheckFile(incomingData, validServerFileName, validLoggerName, maxFileSize, clientCount, cancellationToken);
                    await Logger.WriteLoggerAsync("[SERVER Received]: " + incomingData, validLoggerName, cancellationToken);

                    if (isFull)
                    {
                        await SendResponseAsync(stream, "FULL\n", cancellationToken);
                        break; 
                    }
                    else
                    {
                        await SendResponseAsync(stream, "ack\n", cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                await Logger.WriteLoggerAsync("[SERVER] Cancelation: " + ex, validLoggerName, cancellationToken);
            }
            catch (Exception ex)
            {
                await Logger.WriteLoggerAsync("[SERVER] Error: " + ex, validLoggerName, cancellationToken);
            }
            finally
            {
                Interlocked.Decrement(ref clientCount);
                stream.Close();
                client.Close();
            }
        }
        /// <summary>
        /// A method to send a response to the server
        /// </summary>
        /// <param name="stream">The stream to send the message back to the client</param>
        /// <param name="response">the response message</param>
        /// <param name="token">Cancellation token if need to shutdown tasks</param>
        /// <returns>Task that represent async to send response to client</returns>
        private static async Task SendResponseAsync(NetworkStream stream, string response, CancellationToken token)
        {
            
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length, token);
        }
    }
}
