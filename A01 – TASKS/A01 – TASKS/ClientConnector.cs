//
// FILE               : ClientConnector.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER         : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Client establishes a TCP connection to the server, sends FILESIZE + data,
//                      receives an ack after each message. Multiple clients run concurrently via Tasks.
//
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A01___TASKS
{
    internal class ClientConnector
    {
        MessageSender sender = new MessageSender();
        public async Task RunAsync()
        {
            int clientCount = int.Parse(ConfigurationManager.AppSettings["ClientCount"] ?? "1");
            int messageLength = int.Parse(ConfigurationManager.AppSettings["MessageLength"] ?? "12");

            string ipAddress = ConfigurationManager.AppSettings["Ipaddress"] ?? "127.0.0.1";
            string port = ConfigurationManager.AppSettings["Port"] ?? "14000";
            string sizeDoc = ConfigurationManager.AppSettings["size"] ?? "0";

            Task[] tasks = new Task[clientCount];

            for (int i = 0; i < clientCount; i++)
            {
                int clientId = i + 1;
                tasks[i] = RunSingleClientAsync(clientId, ipAddress, port, sizeDoc, messageLength);
            }

            await Task.WhenAll(tasks);
        }

        private async Task RunSingleClientAsync(int clientId, string ipAddress, string port, string sizeDoc, int messageLength)
        {
            TcpClient? client = null;

            try
            {
                client = await ConnectToServerAsync(ipAddress, port);

                if (client == null)
                {
                    Console.WriteLine($"[Client {clientId}] Failed to connect.");
                    return;
                }

                // 1) Send FILESIZE
                string fileSizeMsg = $"FILESIZE {sizeDoc}";
                await sender.SendAsync(client, fileSizeMsg);
                string ack1 = await sender.ReceiveAsync(client);
                Console.WriteLine($"[Client {clientId}] Sent: {fileSizeMsg} | Server: {ack1}");

                // 2) Send DATA
                string dataMsg = RandomString(messageLength);
                await sender.SendAsync(client, dataMsg);
                string ack2 = await sender.ReceiveAsync(client);
                Console.WriteLine($"[Client {clientId}] Sent data ({dataMsg.Length} bytes) | Server: {ack2}");

                // Optional: end this connection politely (server must handle it)
                await sender.SendAsync(client, "STOP");
                string ack3 = await sender.ReceiveAsync(client);
                Console.WriteLine($"[Client {clientId}] Sent STOP | Server: {ack3}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client {clientId}] Error: {ex.Message}");
            }
            finally
            {
                client?.Close();
            }
        }

        private async Task<TcpClient?> ConnectToServerAsync(string ipAddress, string port)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);
                int portInt = int.Parse(port);

                TcpClient client = new TcpClient();
                await client.ConnectAsync(ip, portInt);
                return client;
            }
            catch
            {
                return null;
            }
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
