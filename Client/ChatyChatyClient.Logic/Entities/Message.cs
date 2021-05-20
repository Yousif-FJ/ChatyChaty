using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Entities
{
    public class Message
    {
        public string Body { get; set; }
        public string SenderName { get; set; }
        public string Id { get; set; }
        public bool IsDelivered { get; set; }
    }
}
