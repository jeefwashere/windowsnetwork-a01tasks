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
            ClientConnector connector = new ClientConnector();
            connector.ClientConnectorServer(ipaddress, port, sizeDoc);

        }


        public void ClientConnectorServer(string ipAddress, string port, string messageBeenSend)
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
                do
                {
                    stream.Write(data, 0, data.Length);
                }
                while (!receiveMessage(client));
                {
                    sendMessage(client, RandomString(12));
                }
 
 
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void sendMessage(TcpClient client, string messageBeenSend)
        {
            NetworkStream stream = null;
            byte[] data = Encoding.UTF8.GetBytes(messageBeenSend);
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            
            if (receiveMessage(client) == false)
            { return; }
            else{ stream.Write(data, 0, data.Length); };
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
