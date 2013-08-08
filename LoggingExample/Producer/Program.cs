using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static void Main()
        {
            const string exchange = "direct-exchange-example";

            Thread.Sleep(1000);
            var connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            var value = DoSomethingInteresting();
            var logMessage = string.Format("{0}: {1}", TraceEventType.Information, value);

            var message = Encoding.UTF8.GetBytes(logMessage);
            
            channel.BasicPublish(exchange, string.Empty, null, message);
            
            channel.Close();
            connection.Close();
        }

        private static string DoSomethingInteresting()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
