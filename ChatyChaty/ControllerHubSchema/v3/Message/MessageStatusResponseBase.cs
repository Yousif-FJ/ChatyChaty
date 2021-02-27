using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public record MessageStatusResponseBase(string MessageId, string ChatId, bool Delievered);
}
