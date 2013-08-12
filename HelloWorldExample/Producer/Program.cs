using System;
using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static void Main()
        {
            const string queue = "hello-world-queue";
            var connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.QueueDeclare(queue, false, false, false, null);

            byte[] message = Encoding.UTF8.GetBytes("Hello, World!");
            channel.BasicPublish(string.Empty, queue, null, message);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            channel.Close();
            connection.Close();
        }
    }
}
