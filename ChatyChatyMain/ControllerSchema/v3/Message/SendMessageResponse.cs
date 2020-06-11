using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v3
{
    public class SendMessageResponse : ResponseBase
    {
        public new MessageInfoBase Data { get; set; }
    }
}
