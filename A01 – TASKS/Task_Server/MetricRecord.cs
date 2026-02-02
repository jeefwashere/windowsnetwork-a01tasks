//
// FILE               : MetricRecord.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Represents a metric entry
//
// Name               : MetricRecord.cs            
// Purpose            : Represents a metric entry
//  
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    internal class MetricRecord
    {
        public int ClientCount { get; set; }
        public long MessageSize { get; set; }
        public int BufferSize { get; set; }
        public long MaxFileSize { get; set; }
        public long WriteTime {  get; set; }
        public DateTime Timestamp {  get; set; }

        public MetricRecord(int clientCount, long messageSize, int bufferSize, long maxFileSize, long writeLatencyMs, DateTime timestamp)
        {
            ClientCount = clientCount;
            MessageSize = messageSize;
            BufferSize = bufferSize;
            MaxFileSize = maxFileSize;
            WriteTime = writeLatencyMs;
            Timestamp = timestamp;
        }
    }
}
