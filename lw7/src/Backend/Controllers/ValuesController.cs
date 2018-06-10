using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Threading;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
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
            var db = new DBHandler();
            string value = db.GetFromDB(id);
            return GetStatusCode(value);
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
            var db = new DBHandler();
            var id = Guid.NewGuid().ToString();
            db.SendToBD(id, value);
            SendToQueue(id);
            return id;
        }

        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {                                     
            var db = new DBHandler();
            string value = db.GetStatisticsFromDB();
            return Ok(value);
        }
    }
}
