using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v3
{
    public class NewMessagesResponse : ResponseBase
    {
        public new IEnumerable<MessageInfoBase> Data { get; set; }
    }
}
