using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static void Main()
        {
            const string exchange = "direct-exchange-example";

            var connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            channel.QueueDeclare("logs", false, false, true, null);
            channel.QueueBind("logs", exchange, string.Empty);

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume("logs", true, consumer);

            var eventArgs = (BasicDeliverEventArgs) consumer.Queue.Dequeue();

            string message = Encoding.UTF8.GetString(eventArgs.Body);
            Console.WriteLine(message);

            channel.Close();
            connection.Close();
            Console.ReadLine();
        }
    }
}
