using System;
using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    public class RabbitLogger:ILogger, IDisposable
    {
        private readonly long _clientId;
        private readonly IModel _channel;
        private bool _disposed;
        private const string Exchange = "topic-exchange-example";

        public RabbitLogger(IConnection connection, long clientId)
        {
            _clientId = clientId;
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(Exchange, ExchangeType.Topic, false, true, null);
        }

        public void Write(Sector sector, string entry, TraceEventType traceEventType)
        {
            byte[] message = Encoding.UTF8.GetBytes(entry);
            var routingKey = string.Format("{0}.{1}.{2}", _clientId, sector.ToString(), traceEventType.ToString());
            _channel.BasicPublish(Exchange, routingKey, null, message);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_channel != null && _channel.IsOpen)
                {
                    _channel.Close();
                }
            }
            GC.SuppressFinalize(this);
        }

        // Destructor
        ~RabbitLogger()
        {
            Dispose();
        }
    }
}