using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;

namespace Producer
{
    class Program
    {
        private const string ExchangeName = "header-exchange-example";

        static void Main()
        {
            var connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();

            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Headers, false, true, null);
            byte[] message = Encoding.UTF8.GetBytes("Hello, World!");

            var properties = new BasicProperties
                {
                    Headers = new Dictionary<string, object>
                        {
                            {"key1", "12345"}
                            ,{"key2", "123455"}
                        }
                };

            var time = TimeSpan.FromSeconds(10);
            var stopwatch = new Stopwatch();
            Console.WriteLine("Running for {0} seconds", time.ToString("ss"));
            stopwatch.Start();
            var messageCount = 0;

            while (stopwatch.Elapsed < time)
            {
                channel.BasicPublish(ExchangeName, "", properties, message);
                messageCount++;
                Console.Write("Time to complete: {0} seconds - Messages published: {1}\r", (time - stopwatch.Elapsed).ToString("ss"), messageCount);
                Thread.Sleep(1000);
            }

            Console.Write(new string(' ', 70) + "\r");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            message = Encoding.UTF8.GetBytes("quit");
            channel.BasicPublish(ExchangeName, "", properties, message);
            channel.Close();
            connection.Close();
        }
    }
}
