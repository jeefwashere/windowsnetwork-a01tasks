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
        /// <summary>
        /// A class to write to the contents to a file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="content">The content to writen</param>
        /// <returns>Task that represent async to write to file</returns>
        public async Task WriteToFileAsync(string path, string content)
        {
            await File.AppendAllTextAsync(path, content);
        }
    }
}
