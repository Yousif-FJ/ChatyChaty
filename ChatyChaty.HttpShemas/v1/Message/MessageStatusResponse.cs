using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Message
{
    public record MessageStatusResponse(string MessageId, string ChatId, bool Delievered);
}
