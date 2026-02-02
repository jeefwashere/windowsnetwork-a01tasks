//
// FILE               : MessageProcessor.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : Processes messages and writes to file safely (thread-safe using lock).
//
// Name               : MessageProcessor.cs            
// Purpose            : Processor message and using aysnc tasks to write to a file 

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Task_Server;

namespace A01___TASKS
{
    internal class MessageProcessor
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1); // Found this on: https://stackoverflow.com/questions/20084695/lock-and-async-method-in-c-sharp
        private readonly Metrics metrics;
        private readonly int bufferSize;
        private static Stopwatch fileFillTimer = new Stopwatch();
        private static bool timerStarted = false;
        private static bool fileLimitReached = false;

        public MessageProcessor(Metrics metrics, int bufferSize)
        {
            this.metrics = metrics;
            this.bufferSize = bufferSize;
        }

        /// <summary>
        /// A method to write to check file and write to file
        /// </summary>
        /// <param name="message">Message to be written</param>
        /// <param name="filePath">File path to be written to</param>
        /// <param name="logFileName">Log file name</param>
        /// <param name="maxFileSize">max file size for server</param>
        /// <returns>A task that represents writing async to a file</returns>
        public async Task<bool> CheckFile(
    string message,
    string filePath,
    string metricsLogFileName,
    string logFileName,
    long maxFileSize,
    int currentClientCount,
    CancellationToken cancellationToken)
        {
            FileIO fileIO = new FileIO();
            bool shouldTerminate = false;

            if (string.IsNullOrEmpty(filePath))
                filePath = "output.txt";

            if (string.IsNullOrEmpty(logFileName))
                logFileName = "logger.txt";

            if (string.IsNullOrEmpty(metricsLogFileName))
                metricsLogFileName = "metrics.txt";

            // Terminal state check
            if (fileLimitReached)
            {
                shouldTerminate = true;
            }
            else
            {
                await semaphoreSlim.WaitAsync(cancellationToken);
                try
                {
                    if (!File.Exists(filePath))
                    {
                        using (File.Create(filePath)) { }
                    }

                    FileInfo fileInfo = new FileInfo(filePath);
                    fileInfo.Refresh();

                    long messageBytes = Encoding.UTF8.GetByteCount(message);
                    long projectedSize = fileInfo.Length + messageBytes;

                    // FULL condition
                    if (maxFileSize > 0 && projectedSize >= maxFileSize)
                    {
                        fileLimitReached = true;
                        fileFillTimer.Stop();

                        long totalTimeMs = fileFillTimer.ElapsedMilliseconds;

                        await Logger.WriteLoggerAsync(
                            $"[METRICS] Time to reach max file size: {totalTimeMs}ms",
                            metricsLogFileName,
                            cancellationToken);

                        shouldTerminate = true;
                    }
                    else
                    {
                        // Start timer on first real write
                        if (!timerStarted)
                        {
                            fileFillTimer.Start();
                            timerStarted = true;
                        }

                        long elapsedTime = await fileIO.WriteToFileAsync(
                            fileInfo.FullName,
                            message,
                            cancellationToken);

                        MetricRecord record = new MetricRecord(
                            currentClientCount,
                            messageBytes,
                            bufferSize,
                            maxFileSize,
                            elapsedTime,
                            DateTime.UtcNow);

                        metrics.Record(record);

                        await Logger.WriteLoggerAsync(
                            @$"[{record.Timestamp}]
                            Current Client Count:   {record.ClientCount}
                            Message Size:           {record.MessageSize}B
                            Buffer Size:            {record.BufferSize}B
                            Max File Size:          {record.MaxFileSize}B
                            Write Time:             {record.WriteTime}ms",
                            metricsLogFileName,
                            cancellationToken);

                        await Logger.WriteLoggerAsync(
                            "Message Received: " + message,
                            logFileName,
                            cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    await Logger.WriteLoggerAsync(
                        "Exception Received: " + ex.Message,
                        logFileName,
                        cancellationToken);

                    shouldTerminate = true;
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }

            return shouldTerminate;
        }

    }
}