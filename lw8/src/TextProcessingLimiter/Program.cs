using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TextProcessingLimiter
{
    class Program
    {
        private static int maxTextCount = 3;

        private static void SendToQueue(string value, string status)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("processing-limiter", "direct");

                string message = "ProcessingAccepted:" + value + ":" + status;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "processing-limiter",
                                    routingKey: "processing-limited",
                                    basicProperties: null,
                                    body: body
                );
            }
        }
        
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("backend-api", "fanout");
                channel.ExchangeDeclare("text-success-marker", "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                var successMarkerQueueName = channel.QueueDeclare().QueueName;
                while (true)
                {
                    channel.QueueBind(queue: queueName,
                                    exchange: "backend-api",
                                    routingKey: "");

                    channel.QueueBind(queue: successMarkerQueueName,
                                    exchange: "text-success-marker",
                                    routingKey: "");
                
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var data = Regex.Split(message, ":");

                        if (data[0] == "TextCreated")
                        {
                            if( maxTextCount > 0)
                            {
                                --maxTextCount;
                                Console.WriteLine("Max text count --");
                                Console.WriteLine("Text received: " + message);
                                SendToQueue(data[1], "true");
                            }
                            else
                            {
                                Console.WriteLine("Limit exceeded");
                                SendToQueue(data[1], "false");
                            }
                        }

                        if (data[0] == "TextSuccessMarked" && data[2] == "false")
                        {
                            ++maxTextCount;
                            Console.WriteLine("Max text count ++");
                            Console.WriteLine("Text rejected");
                        }
                    };
                    
                    channel.BasicConsume(queue: queueName,
                                        autoAck: true,
                                        consumer: consumer
                    );

                    channel.BasicConsume(queue: successMarkerQueueName,
                                        autoAck: true,
                                        consumer: consumer
                    );
                }
            }
        }
    }
}
