using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Threading;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private string GetFromDB(string id)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase();
            string value = db.StringGet(id);
            int tryNumber = 10;
            int sleepTime = 200;

            for (int i = 0; i <= tryNumber; ++i)
            {
                if (value != null)
                {
                    try
                    {
                        float tmpValue = (float) Convert.ToDouble(value);
                        break;
                    }
                    catch(FormatException)
                    {

                    }
                }

                Thread.Sleep(sleepTime);
                value = db.StringGet(id);
            }

            return value;
        }

        private IActionResult GetStatusCode(string value)
        {
            IActionResult result = null;
            if (value != null) 
            {
                result = Ok(value);
            }
            else
            {
                result = new NotFoundResult();
            }
            return result;
        }

        // GET api/values/<id>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            string value = GetFromDB(id);
            return GetStatusCode(value);
        }

        private void SendToBD(string id, string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase();

            db.StringSet(id, value);
        }

        private void SendToQueue(string id)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("backend-api", "fanout");

                string message = "TextCreated:" + id;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "backend-api",
                                    routingKey: "",
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
