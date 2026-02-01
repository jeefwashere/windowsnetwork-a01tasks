//
// FILE               : MainProgramServer.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is the server starts running
// 
// Name               : MainProgramServer            
// Purpose            : The enrty point for the server to run

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

            Console.CancelKeyPress += (sender, e) => //Might change
            {
                e.Cancel = true;
                cts.Cancel();
            };

            ServerAsync sr = new ServerAsync();
            await sr.RunAsync(cts.Token);
        }
    }
}
