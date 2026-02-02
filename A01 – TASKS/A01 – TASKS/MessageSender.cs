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
    {/// <summary>
     /// send message to server
     /// </summary>
     /// <param name="client">whicit client</param>
     /// <param name="message">what we send</param>
     /// <returns></returns>
        public async Task SendAsync(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();

            //this just make sure we read whole message
            if (!message.EndsWith("\n"))
            {
                message += "\n";
            }

            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }
        /// <summary>
        /// this is for recfeive  message from server
        /// </summary>
        /// <param name="client">client we recivce</param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public async Task<string> ReceiveAsync(TcpClient client)
        {
            string result = string.Empty;
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];
                //get the strea to read
                while (true)
                {   // read them
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {//no message
                        throw new IOException("Remote closed the connection.");
                    }
                    // this to test did we read to the end
                    result += Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (result.Contains("\n"))
                    {
                        break; // got at least one full message
                    }
                    //write into console
                    Console.WriteLine($"RESULT: {result}");
                    Console.WriteLine($"RECEVIED: {bytesRead}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            //split [0] means the part before \n and trim to remove space
            return result.Split('\n')[0].Trim();
        }
    }
}
