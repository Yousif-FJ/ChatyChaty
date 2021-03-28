using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.HttpSchemas.Message
{
    public class MessageInfoReponseBase
    {
        public MessageInfoReponseBase()
        {

        }
        public string ChatId { get; set; }
        public string MessageId { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
        public bool? Delivered { get; set; }
    }
}
