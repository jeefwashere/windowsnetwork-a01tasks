//
// FILE               : MessageSender.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Sends and receives TCP messages for the client.
//
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace A01___TASKS
{
    internal class MessageSender
    {
        public async Task SendAsync(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();

 
            if (!message.EndsWith("\n"))
            {
                message += "\n";
            }

            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public async Task<string> ReceiveAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            string result = string.Empty;

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    throw new IOException("Remote closed the connection.");
                }

                result += Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (result.Contains("\n"))
                {
                    break; // got at least one full message
                }
            }


            return result.Split('\n')[0].Trim();
        }
    }
}
