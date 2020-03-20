using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v2
{
    public class GetNewMessagesResponse
    {
        public long ConversationId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }

    public class Message
    {
        public long MessageId { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
    }
}
