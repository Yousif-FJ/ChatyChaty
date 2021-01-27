using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public record MessageStatusResponseBase(long MessageId, long ChatId, bool Delievered);
}
