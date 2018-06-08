using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TextRankCalc
{
    class Program
    {
        private static HashSet<char> vowels = new HashSet<char>(new char[] { 'a', 'e', 'i', 'o', 'u', 'y' });
        private static HashSet<char> consonants = new HashSet<char>(new char[] { 'b', 'c', 'd', 'f', 'g',
         'h','j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'});

        private static void SendRankToDB(string id, float value) 
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase();
                                    
            db.StringSet(id, value);            
        }

        private static float CalcRank(string text)
        {
            float vowelCount = 0;
            float consonantCount = 0;
            for(int i = 0; i < text.Length; i++)
            {
                if(vowels.Contains(text[i]))
                {
                    vowelCount++;
                }
                else if (consonants.Contains(text[i]))
                {
                    consonantCount++;
                }
            }

            return (consonantCount == 0) ? vowelCount : vowelCount / consonantCount;
        }

        private static string GetValueById(string id) 
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase();

            return db.StringGet(id);
        }
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("backend-api", "fanout");                
                var queueName = channel.QueueDeclare().QueueName;
                while (true)
                {
                    channel.QueueBind(queue: queueName,
                                    exchange: "backend-api",
                                    routingKey: "");
                
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        string id = Regex.Split(message, ":")[1];
                        string value = GetValueById(id);
                        float rank = CalcRank(value);
                        SendRankToDB(id, rank);
                        Console.WriteLine(id + " : " + rank);
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