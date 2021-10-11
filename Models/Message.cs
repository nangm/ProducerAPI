using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProducerAPI.Models
{
    public class Message
    {
        public string email { get; set; }

        public string password { get; set; }
        public string task { get; set; }
    }
}
