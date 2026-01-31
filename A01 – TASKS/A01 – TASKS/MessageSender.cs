//
// FILE               : MessageSender.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is where the client will send a message to the server
// 
// Name               : MessageSender            
// Purpose            : A message will be generated and sent to the server
//                      
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A01___TASKS
{
    internal class MessageSender
    {
        public async Task SendAsync(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public async Task<string> ReceiveAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            string result = "";
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if(bytesRead > 0)
            {
                result = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            return result;
        }
    }
}
