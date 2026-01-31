//
// FILE               : ClientConnector.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is where the client establish a connection to the server
// 
// Name               : Client            
// Purpose            : The client will establish a connection using the TCP/IP reading the 
//                      IP and port from a config file.
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace A01___TASKS
{

    internal class ClientConnector
    {


        static void Main(string[] args)
        {
            int clientCount = int.Parse(
                ConfigurationManager.AppSettings["ClientCount"]
            );

            int messageLength = int.Parse(
                ConfigurationManager.AppSettings["MessageLength"]
            );
            string ipaddress = ConfigurationManager.AppSettings["Ipaddress"];
            string port = ConfigurationManager.AppSettings["Port"];
            string sizeDoc = ConfigurationManager.AppSettings["size"];
            Console.WriteLine("ClientCount = " + clientCount);
            Console.WriteLine("MessageLength = " + messageLength);
            ClientConnector clientConnector = new ClientConnector();
            TcpClient client =clientConnector.ClientConnectorServer(ipaddress, port, sizeDoc);
            if(client == null)
            {
                Console.WriteLine("Failed to connect to server.");
                return;
            }

            try
            {
                // Run multiple tasks that may throw exceptions
                Task task = Task.WhenAll(
                    Task.Run(() => clientConnector.SendOnce(client, clientConnector.RandomString(12))),
                    Task.Run(() => clientConnector.receiveMessage(client))



                );

                // Wait for all tasks to complete (exceptions will be aggregated)
                task.Wait();
            }
            catch (AggregateException aggEx)
            {
                Console.WriteLine("AggregateException caught. Processing inner exceptions...\n");

                // Handle each exception individually
                aggEx.Handle(ex =>
                {
                    if (ex is ArgumentNullException)
                    {
                        Console.WriteLine($"Handled ArgumentNullException: {ex.Message}");
                        return true; // Mark as handled
                    }
                    else if (ex is ArgumentOutOfRangeException)
                    {
                        Console.WriteLine($"Handled ArgumentOutOfRangeException: {ex.Message}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Unhandled exception type: {ex.GetType().Name} - {ex.Message}");
                        return false; // Not handled, will be rethrown
                    }
                });
            }
        }


        public TcpClient ClientConnectorServer(string ipAddress, string port, string messageBeenSend)
        {

            NetworkStream stream = null;
            IPAddress iPAddress = IPAddress.Parse(ipAddress);
            int portInt = int.Parse(port);

            byte[] data = Encoding.UTF8.GetBytes(messageBeenSend);
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(iPAddress, portInt);
                //this was where to create the stream can be write and recipte the message
                stream = client.GetStream();
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public bool receiveMessage(TcpClient client)
        {
            bool continueReceive = true;
            NetworkStream stream = null;
            byte[] receiveData = new byte[4096];
            stream = client.GetStream();
            int bytesRead = stream.Read(receiveData, 0, receiveData.Length);
            string response = Encoding.ASCII.GetString(receiveData, 0, bytesRead);
            if (response.Contains("Stop") ){
                continueReceive= false; 
            };
            return continueReceive;
            

 

        }
        public bool SendOnce(TcpClient client, string msg)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream.Write(data, 0, data.Length);
            return true; 
        }
        public string RandomString(int length)
        {
;           
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length was error must big than 0");
                    
            };

            string stringSen = "";
            for (int i = 0; i < length; i++)
            {
                stringSen = stringSen + 'c';
            }
            return stringSen;
        }

    }

}
