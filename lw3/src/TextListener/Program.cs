using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Text;

namespace TextListener
{
    class Program
    {
        private static string GetValueById(string id) 
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase();

            return db.StringGet(id);
        }

        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                while (true)
                {
                    channel.QueueDeclare(queue: "backend-api",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null
                    );

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var id = Encoding.UTF8.GetString(body);
                        string value = GetValueById(id);
                        Console.WriteLine(id + " : " + value);
                    };
                    channel.BasicConsume(queue: "backend-api",
                                        autoAck: true,
                                        consumer: consumer
                    );

                }
            }
        }
    }
}