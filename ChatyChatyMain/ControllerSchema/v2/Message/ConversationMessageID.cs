using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v2
{
    public class ConversationMessageID
    {
        public long ConversationId { get; set; }
        public long LastMessageId { get; set; }
    }
}
