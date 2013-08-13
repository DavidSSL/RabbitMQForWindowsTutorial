using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Consumer
{
    class Program
    {
        private const string QueueName = "header-queue-example";
        private const string ExchangeName = "header-exchange-example";

        static void Main()
        {
            var connectionFactory = new ConnectionFactory();

            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Headers, false, true, null);
            channel.QueueDeclare(QueueName, false, false, true, null);

            IDictionary specs = new Dictionary<string, string>();
            specs.Add("x-match", "any");
            specs.Add("key1", "12345");
            specs.Add("key2", "123455");
            channel.QueueBind(QueueName, ExchangeName, string.Empty, specs);

            channel.StartConsume(QueueName, MessageHandler);
            channel.Close();
            connection.Close();
        }

        public static void MessageHandler(IModel channel, DefaultBasicConsumer consumer, BasicDeliverEventArgs eventArgs)
        {
            string message = Encoding.UTF8.GetString(eventArgs.Body);
            Console.WriteLine("Message received: " + message);

            foreach (object headerKey in eventArgs.BasicProperties.Headers.Keys)
            {
                Console.WriteLine(headerKey + ": " + eventArgs.BasicProperties.Headers[headerKey]);
            }

            if (message == "quit")
            {
                channel.BasicCancel(consumer.ConsumerTag);
            }
        }
    }
}
