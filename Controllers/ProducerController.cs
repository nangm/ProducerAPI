using Microsoft.AspNetCore.Mvc;
using ProducerAPI.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProducerAPI.Controllers
{
    [Route("api/[controller]/task")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        public object Newtonsoft { get; private set; }

        [HttpPost]
        public async Task<string> PostAsync(Message msg)
        {
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(" https://reqres.in/api/login");
                var res = await client.PostAsync(client.BaseAddress, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(msg), Encoding.UTF8, "application/json"));
                
                HttpResponseMessage response = new HttpResponseMessage();
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                
                // URL Request parameters.  
                HttpContent requestParams = new FormUrlEncodedContent(allIputParams);
                response = await client.PostAsync("Token", requestParams).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string token = "QpwL5tke4Pnpja7X4";
                }
            }
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
                channel.QueueDeclare(queue: "greetings",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = msg.task;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "greetings",
                                     basicProperties: null,
                                     body: body);
            }
            return Ok();
        }
        public static bool IsTokenValid(string token)
        {
            return (new List<string> { "QpwL5tke4Pnpja7X4"}).Contains(token);
        }

    }
}
