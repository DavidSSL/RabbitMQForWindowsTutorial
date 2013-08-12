using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        private static volatile bool _cancelling;
        private const string Exchange = "fanout-exchange-example";
        static void Main()
        {
            var connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            
            channel.ExchangeDeclare(Exchange, ExchangeType.Fanout);
            
            var thread = new Thread(() => PublishQuotes(channel));
            thread.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            _cancelling = true;

            channel.Close();
            connection.Close();
        }

        private static void PublishQuotes(IModel channel)
        {
            while (true)
            {
                if (_cancelling)
                {
                    return;
                }
                IEnumerable<string> quotes = FetchStockQuotes(new[] { "GOOG", "HD", "MCD" });

                foreach (var quote in quotes)
                {
                    byte[] message = Encoding.UTF8.GetBytes(quote);
                    channel.BasicPublish(Exchange, "", null, message);
                }
                Thread.Sleep(5000);
            }
        }

        private static IEnumerable<string> FetchStockQuotes(string[] symbols)
        {
            var quotes = new List<string>();

            string url = string.Format("http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20({0})&env=store://datatables.org/alltableswithkeys",
                String.Join("%2C", symbols.Select(s => "%22" + s + "%22")));
            var wc = new WebClient { Proxy = WebRequest.DefaultWebProxy };
            var ms = new MemoryStream(wc.DownloadData(url));
            var reader = new XmlTextReader(ms);
            XDocument doc = XDocument.Load(reader);
            XElement results = doc.Root.Element("results");

            foreach (string symbol in symbols)
            {
                XElement q = results.Elements("quote").First(w => w.Attribute("symbol").Value == symbol);
                quotes.Add(symbol + ":" + q.Element("AskRealtime").Value);
            }

            return quotes;
        }
    }
}
