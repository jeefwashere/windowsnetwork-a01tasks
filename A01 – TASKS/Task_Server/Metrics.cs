using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    internal class Metrics
    {
        private readonly ConcurrentBag<MetricRecord> metricsRecords = new ConcurrentBag<MetricRecord>(); // Found the thread-safe collections here: https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/

        public void Record(MetricRecord metrics)
        {
            metricsRecords.Add(metrics);
        }

        public List<MetricRecord> GetMetrics()
        {
            return metricsRecords.ToList();
        }
    }
}
