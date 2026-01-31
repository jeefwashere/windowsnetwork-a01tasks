//
// FILE               : Server.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is where the server establish a connection to the client
// 
// Name               : Server            
// Purpose            : The server will establish a connection to the client using the TCP/IP reading the 
//                      IP and port from a config file.

using System;
using System.Configuration;
using System.Net;

namespace Task_Server
{
    internal class MainProgramServer
    {
        
        static async Task Main(string[] args)
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            ServerAsync server = new ServerAsync();
            await server.RunAsync(cts.Token);
        }
    }
}
