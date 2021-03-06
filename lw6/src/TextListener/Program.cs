﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TextListener
{
    class Program
    {
        private static string GetValueById(string id) 
        {
            int dbIndex = GetDBIndexByMessageHash(id);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase(dbIndex);
            string value = db.StringGet(id);
            Console.WriteLine(id + " got from db" + dbIndex);

            return value;
        }

        private static int GetDBIndexByMessageHash(string msg)
        {
            int hash = 0;
            foreach (Char symbol in msg)
            {
                hash += symbol;
            }
            return hash % 16;
        }
        
        public static void Main(string[] args)
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
                        Console.WriteLine(id + " : " + value);
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