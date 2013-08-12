using System;
using System.Diagnostics;
using System.Threading;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        private const long ClientId = 10843;

        static void Main()
        {
            var connectionFactory = new ConnectionFactory();
            var connection = connectionFactory.CreateConnection();

            var time = TimeSpan.FromSeconds(10);
            var stopwatch = new Stopwatch();
            Console.WriteLine("Running for {0} seconds", time.ToString("ss"));
            stopwatch.Start();

            while (stopwatch.Elapsed < time)
            {
                using (var logger = new RabbitLogger(connection, ClientId))
                {
                    Console.WriteLine("Time to complete: {0} seconds\r", (time- stopwatch.Elapsed).ToString("ss"));
                    logger.Write(Sector.Personal, "This is an information message", TraceEventType.Information);
                    logger.Write(Sector.Business, "This is an warning message", TraceEventType.Warning);
                    logger.Write(Sector.Business, "This is an error message", TraceEventType.Error);
                    Thread.Sleep(1000);
                }
            }
            connection.Close();
            Console.Write("                             \r");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
