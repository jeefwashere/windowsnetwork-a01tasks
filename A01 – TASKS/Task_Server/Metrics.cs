//
// FILE               : Metrics.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Used to stash metrics entries into a concurrent list
//
// Name               : Metrics.cs            
// Purpose            : The class where each entry is added into a list
//  
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

        /// <summary>
        /// A method to add metrics entries to a list
        /// </summary>
        /// <param name="metrics">Each entry of a metric</param>
        /// <returns>Adds metrics to list</returns>
        public void Record(MetricRecord metrics)
        {
            metricsRecords.Add(metrics);
        }

        /// <summary>
        /// Returns all metrics in the list
        /// </summary>
        /// <param name="metrics">Each entry of a metric</param>
        /// <returns>All metrics entries</returns>
        public List<MetricRecord> GetMetrics()
        {
            return metricsRecords.ToList();
        }
    }
}
