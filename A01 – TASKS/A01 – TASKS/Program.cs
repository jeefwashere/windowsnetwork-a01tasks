using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
// FILE               : Program.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : main place to run the client side
//
namespace A01___TASKS
{
    internal class Program
    {
        /// <summary>
        /// place to run the client connector
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            ClientConnector cc = new ClientConnector();
            await cc.RunAsync();

            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadKey();
        }

    }
}
