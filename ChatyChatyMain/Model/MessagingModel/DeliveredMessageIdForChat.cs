using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessagingModel
{
    public class DeliveredMessageIdForChat
    {
        public long ChatId { get; set; }
        public long LastDeliveredMessageId { get; set; }
    }
}
