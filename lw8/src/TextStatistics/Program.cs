using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TextStatistics
{
    class Program
    {
        private static int textNum = 0;
        private static int highRankPart = 0;
        private static float avgRank = 0;
        private static float rankSum = 0;
        private static DBHandler db = new DBHandler();
        private static void UpdateStatistics(string id, string status)
        {
            ++textNum;

            float floatValue = db.GetValueById(id);
            rankSum += floatValue;
            if (status == "true")
            {
                ++highRankPart;
            }
            Console.WriteLine("update statistics1");
            avgRank = rankSum / textNum;
        }

        static void Main(string[] args)
        {
            db.SendStatisticsToBD(textNum, highRankPart, avgRank);
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("text-success-marker", "fanout");                
                var queueName = channel.QueueDeclare().QueueName;
                while (true)
                {
                    channel.QueueBind(queue: queueName,
                                    exchange: "text-success-marker",
                                    routingKey: "");
                
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var data = Regex.Split(message, ":");
                        if (data[0] == "TextSuccessMarked")
                        {
                            string id = data[1];
                            string status = data[2];
                            UpdateStatistics(id, status);
                            Console.WriteLine("update statistics");
                            db.SendStatisticsToBD(textNum, highRankPart, avgRank);
                            Console.WriteLine("current statistics: " + textNum + " : " + highRankPart + " : " + avgRank);
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
