using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A01___TASKS
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ClientConnector cc = new ClientConnector();
            await cc.RunAsync();

            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadKey();
        }

    }
}
