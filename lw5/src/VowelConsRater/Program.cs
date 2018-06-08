using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
namespace VowelConsRater
{
    class Program
    {
        private static void SendRankToDB(string id, float value) 
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase();
                                    
            db.StringSet(id, value);            
        }

        private static float CalcRank(string vowels, string consonant)
        {
            float vowelCount = float.Parse(vowels);
            float consonantCount = float.Parse(consonant);

            return (consonantCount == 0) ? vowelCount : vowelCount / consonantCount;
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
                            float rank = CalcRank(data[2], data[3]);
                            Console.WriteLine(rank);
                            SendRankToDB(id, rank);
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
