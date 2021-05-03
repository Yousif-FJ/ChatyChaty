using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Message
{
    public record MessageInfoReponse(string ChatId, string MessageId, string Sender, string Body, bool? Delivered);
}
