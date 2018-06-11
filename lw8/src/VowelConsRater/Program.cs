using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace VowelConsRater
{
    class Program
    {
        static private float CalcRank(string vowels, string consonant)
        {
            float vowelCount = float.Parse(vowels);
            float consonantCount = float.Parse(consonant);

            return (consonantCount == 0) ? vowelCount : vowelCount / consonantCount;
        }

        static private void SendRankToQueue(string id, float value)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("text-rank-calc", "fanout");

                string message = "TextRankCalculated:" + id + ":" + value;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "text-rank-calc",
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
                channel.ExchangeDeclare("vowel-cons-counter", "direct");                
                var queueName = channel.QueueDeclare().QueueName;
                while (true)
                {
                    channel.QueueBind(queue: queueName,
                                    exchange: "vowel-cons-counter",
                                    routingKey: "vowel-cons-counted");
                
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var data = Regex.Split(message, ":");
                        if (data[0] == "VowelConsCounted")
                        {
                            string id = data[1];
                            var db = new DBHandler();
                            float rank = CalcRank(data[2], data[3]);
                            Console.WriteLine(rank);
                            db.SendToDB(id, rank);
                            SendRankToQueue(id, rank);
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
