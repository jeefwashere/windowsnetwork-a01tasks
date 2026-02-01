using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Server
{
    internal class Metrics
    {
        private readonly List<MetricRecord> metricsRecords = new List<MetricRecord>();

        public void Record(MetricRecord metrics)
        {
            metricsRecords.Add(metrics);
        }

        public ReadOnlyCollection<MetricRecord> GetMetrics()
        {
            return metricsRecords.AsReadOnly(); // Return list as readonly list to make data incorruptible: https://stackoverflow.com/questions/5742726/make-list-immutable
        }
    }
}
