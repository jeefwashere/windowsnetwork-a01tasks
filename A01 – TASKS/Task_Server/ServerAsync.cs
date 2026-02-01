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

        bool metricsClientCountValidStart = false;
        bool metricsFileSizeValidStart = false;

        ValidationClass validator = new ValidationClass();
        Parser parser = new Parser();
        MessageProcessor processor = new MessageProcessor();
        Metrics metrics = new Metrics();

        /// <summary>
        /// A method to run the server
        /// </summary>
        /// <param name="cancellationToken">Cancellation token if need to shutdown task</param>
        /// <returns> Task that represent async start  the server</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await GetConfigInfo(); //Get Info from app config
            await ClientConnectionAsync(cancellationToken);
        }
        /// <summary>
        /// A method to get the config info and assing it to the class varibales
        /// </summary>
        /// <returns>Task that represent async getting appconfig</returns>
        public async Task GetConfigInfo()
        {
            string address = ConfigurationManager.AppSettings["IPAddress"] ?? "127.0.0.1";
            validIP = validator.ValidateIp(address);

            validPort = validator.ValudatePort();
            if (validPort == KInvalidPort)
            {
                await Logger.WriteLoggerAsync("[SERVER CONFIG] Invalid port detected.", validLoggerName);
            }

            validServerFileName = ConfigurationManager.AppSettings["ServerFileName"] ?? string.Empty;
            validLoggerName = ConfigurationManager.AppSettings["LoggerFileName"] ?? string.Empty;
            validMetricsLoggerName = ConfigurationManager.AppSettings["MetricsLoggerFileName"] ?? string.Empty;

            await Logger.WriteLoggerAsync("[SERVER CONFIG] IP=" + validIP + " PORT=" + validPort,validLoggerName);
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
                    await Logger.WriteLoggerAsync("[SERVER] Client connected", validLoggerName);
                    _ = ProcessRequest(client, cancellationToken); //Ignore return task
                }

                catch (SocketException ex)
                {
                    await Logger.WriteLoggerAsync("[SERVER] Socket Exception: " + ex, validLoggerName);
                }
                catch (Exception ex)
                {
                    await Logger.WriteLoggerAsync("[SERVER] Accept error: " + ex, validLoggerName);
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

                    if (incomingData.StartsWith("CLIENTCOUNT"))
                    {
                        clientCount = parser.ParseClientCount(incomingData);
                        if (clientCount > 0)
                        {
                            metricsClientCountValidStart = true;
                        }
                        await SendResponseAsync(stream, "ack\n", cancellationToken); // Debatable if needed?
                        continue;
                    }

                    
                    if (incomingData.StartsWith("FILESIZE"))
                    {
                        maxFileSize = parser.ParseFileSizeMessage(incomingData);
                        if (maxFileSize > 0)
                        {
                            metricsFileSizeValidStart = true;
                        }
                        await SendResponseAsync(stream, "ack\n", cancellationToken);
                        continue;
                    }

                    if (metricsClientCountValidStart && metricsFileSizeValidStart)
                    {
                        _ = metrics.MeasureFileWriteTime(
                                clientCount,
                                messageSize,
                                BUFFER_SIZE,
                                Stopwatch.StartNew(),
                                validServerFileName,
                                maxFileSize,
                                validLoggerName
                            );

                        metricsClientCountValidStart = false;
                        metricsFileSizeValidStart = false;
                    }

                    bool isFull = await processor.CheckFile(incomingData, validServerFileName, validLoggerName, maxFileSize);
                    await Logger.WriteLoggerAsync("[SERVER Received]: " + incomingData, validLoggerName);

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
                await Logger.WriteLoggerAsync("[SERVER] Cancelation: " + ex, validLoggerName);
            }
            catch (Exception ex)
            {
                await Logger.WriteLoggerAsync("[SERVER] Error: " + ex, validLoggerName);
            }
            finally
            {
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
