using System;
using System.IO;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer2
{
    class Program
    {
        private const string Exchange = "fanout-exchange-example";
        private const string Queue = "quotes2";

        static void Main()
        {
            var connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(Exchange, ExchangeType.Direct);

            channel.QueueDeclare(Queue, false, false, true, null);
            channel.QueueBind(Queue, Exchange, "anything");

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(Queue, true, consumer);

            Console.WriteLine("In Consumer2");
            Console.WriteLine("===========");
            while (true)
            {
                try
                {
                    var eventArgs = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    var message = Encoding.UTF8.GetString(eventArgs.Body);
                    Console.WriteLine(message);
                    Console.WriteLine("--------");
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
