using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Message
{
    public record MessageResponse(string ChatId, string MessageId, string Sender, string Body, DateTime SentTime, DateTime? DeliveryTime, bool? Delivered);
}
