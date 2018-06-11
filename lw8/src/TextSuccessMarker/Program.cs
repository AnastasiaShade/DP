using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TextSuccessMarker
{
    class Program
    {
        private static float minSuccessValue = 0.5f;

        private static void SendToQueue(string id, string status)
        {        
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("text-success-marker", "fanout");

                string message = "TextSuccessMarked:" + id + ":" + status;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "text-success-marker",
                                    routingKey: "",
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
                channel.ExchangeDeclare("text-rank-calc", "fanout");                
                var queueName = channel.QueueDeclare().QueueName;
                while (true)
                {
                    channel.QueueBind(queue: queueName,
                                    exchange: "text-rank-calc",
                                    routingKey: "");
                
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var data = Regex.Split(message, ":");
                        if (data[0] == "TextRankCalculated")
                        {
                            string id = data[1];
                            float rank = float.Parse(data[2]);                     
                            if (rank > minSuccessValue)
                            {
                                SendToQueue(data[1], "true");
                            } 
                            else
                            {
                                SendToQueue(data[1], "false");
                            }
                            Console.WriteLine(message);
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
