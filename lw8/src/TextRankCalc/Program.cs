using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TextRankCalc
{
    class Program
    {
        private static void SendToQueue(string id)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("text-rank-tasks", "direct");

                string message = "TextRankTask:" + id;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "text-rank-tasks",
                                    routingKey: "text-rank-task",
                                    basicProperties: null,
                                    body: body
                );
            }
        }

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("processing-limiter", "direct");                
                var queueName = channel.QueueDeclare().QueueName;
                while (true)
                {
                    channel.QueueBind(queue: queueName,
                                    exchange: "processing-limiter",
                                    routingKey: "processing-limited");
                
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var data = Regex.Split(message, ":");
                        if (data[0] == "ProcessingAccepted" && data[2] == "true")
                        {
                            Console.WriteLine(message);
                            string id = data[1];
                            SendToQueue(id);
                        }
                    };
                    
                    channel.BasicConsume(queue: queueName,
                                        autoAck: true,
                                        consumer: consumer
                    );
                }
            }
        }
    }
}