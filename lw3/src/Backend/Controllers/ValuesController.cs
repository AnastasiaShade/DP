using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

            return db.StringGet(id);
        }

        private void SendToBD(string id, string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

            db.StringSet(id, value);         
        }
        private void SendToQueue(string id)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "backend-api",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null
                );

                string message = id;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                    routingKey: "backend-api",
                                    basicProperties: null,
                                    body: body
                );                
            }            
        }

        // POST api/values
        [HttpPost]
        public string Post([FromForm] string value)
        {         
            var id = Guid.NewGuid().ToString();
            SendToBD(id, value);
            SendToQueue(id);
            return id;            
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
