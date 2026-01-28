//
// FILE               : FileIO.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : This file is where all file operation is done
// 
// Name               : FileIO.cs            
// Purpose            : Where the file is open, closed and written to
//            
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Task_Server
{
    internal class FileIO
    {
        public async Task WriteToFileAsync(string path, string content)
        {
            await File.AppendAllTextAsync(path, content);
        }
    }
}
