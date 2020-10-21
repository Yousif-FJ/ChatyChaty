using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.MessagingModel
{
    public class NewConversationModel
    {
        public ConversationInfo Conversation { get; set; }
        public string Error { get; set; }
    }
}
