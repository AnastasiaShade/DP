using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace VowelConsCounter
{
    class Program
    {
        private static int vowelCount = 0;
        private static int consonantCount = 0;
        private static HashSet<char> vowels = new HashSet<char>(new char[] { 'a', 'e', 'i', 'o', 'u', 'y' });
        private static HashSet<char> consonants = new HashSet<char>(new char[] { 'b', 'c', 'd', 'f', 'g',
            'h','j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'});

        private static void CalcVowelsCons(string text)
        {
            vowelCount = 0;
            consonantCount = 0;
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
        }

        private static void SendToQueue(string id)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("vowel-cons-counter", "direct");

                string message = "VowelConsCounted:" + id + ":" + vowelCount + ":" + consonantCount;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "vowel-cons-counter",
                                    routingKey: "vowel-cons-counted",
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
                channel.ExchangeDeclare("text-rank-tasks", "direct");                
                var queueName = channel.QueueDeclare().QueueName;
                while (true)
                {
                    channel.QueueBind(queue: queueName,
                                    exchange: "text-rank-tasks",
                                    routingKey: "text-rank-task");
                
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var data = Regex.Split(message, ":");
                        if (data[0] == "TextRankTask")
                        {
                            string id = data[1];
                            var db = new DBHandler();
                            string value = db.GetValueById(id);
                            CalcVowelsCons(value);
                            SendToQueue(id);
                            Console.WriteLine(id + ":" + vowelCount + ":" + consonantCount);
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
