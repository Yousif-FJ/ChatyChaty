using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.HttpSchemas.Message
{
    public record MessageStatusResponseBase(string MessageId, string ChatId, bool Delievered);
}
