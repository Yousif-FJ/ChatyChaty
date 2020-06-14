using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v3
{
    public class MessageInfoBase
    {
        public long ChatId { get; set; }
        public long MessageId { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
        public bool? Delivered { get; set; }
    }
}
