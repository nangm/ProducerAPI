using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProducerAPI.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProducerAPI.Controllers
{
    [Route("api/[controller]/task")]
    [ApiController]
    public class ProducerController : ControllerBase
    {        
        [HttpPost]
        public async Task<IActionResult> PostAsync(Message msg)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://reqres.in/api/login");
                var res = await client.PostAsync(client.BaseAddress, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(msg), Encoding.UTF8, "application/json"));            
                      
                
                if (res.IsSuccessStatusCode)
                {
                    var factory = new ConnectionFactory()
                    {
                        //HostName = "localhost" , 
                        //Port = 30724
                        HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                        Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
                    };

                    Console.WriteLine(factory.HostName + ":" + factory.Port);
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "TaskQueue",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        string message = msg.task;
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "TaskQueue",
                                             basicProperties: null,
                                             body: body);
                    }
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }   
        }
    }
}
