﻿using System;
using System.IO;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Should show only Personal messages");
            Console.WriteLine("=================================="); const string exchange = "topic-exchange-example";
            const string queue = "log-personal";

            var connectionFactory = new ConnectionFactory();
            var connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Topic, false, true, null);
            channel.QueueDeclare(queue, false, false, true, null);
            channel.QueueBind(queue, exchange, "*.Personal.*");

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queue, true, consumer);

            while (true)
            {
                try
                {
                    var eventArgs = (BasicDeliverEventArgs) consumer.Queue.Dequeue();
                    var message = Encoding.UTF8.GetString(eventArgs.Body);
                    Console.WriteLine(string.Format("{0} - {1}", eventArgs.RoutingKey, message));
                }
                catch (EndOfStreamException)
                {
                    // The consumer was cancelled, the model closed, or the connection went away.
                    break;
                }
            }
            channel.Close();
            connection.Close();
        }
    }
}
