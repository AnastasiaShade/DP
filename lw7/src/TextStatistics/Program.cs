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

        private static void UpdateStatistics(string value)
        {
            ++textNum;

            float floatValue = (float) Convert.ToDouble(value);
            rankSum += floatValue;
            if (floatValue > 0.5f)
            {
                ++highRankPart;
            }

            avgRank = rankSum / textNum;
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
                            string value = data[2];
                            UpdateStatistics(value);
                            var db = new DBHandler();
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
