using System;
using System.IO;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    public static class ChannelExtensions
    {
         public static void StartConsume(this IModel channel, string queuename,
                                         Action<IModel, DefaultBasicConsumer, BasicDeliverEventArgs> callback)
         {
             var consumer = new QueueingBasicConsumer(channel);
             channel.BasicConsume(queuename, true, consumer);

             while (true)
             {
                 try
                 {
                     var eventArgs = (BasicDeliverEventArgs) consumer.Queue.Dequeue();
                     new Thread(() => callback(channel, consumer, eventArgs)).Start();
                 }
                 catch (EndOfStreamException)
                 {
                     // The consumer was cancelled, the model closed, or the connection went away.
                     break;
                 }
             }
         }
    }
}