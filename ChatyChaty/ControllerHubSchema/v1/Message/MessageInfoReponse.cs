using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v1
{
    public record MessageInfoReponse(string ChatId, string MessageId, string Sender, string Body, bool? Delivered);
}
