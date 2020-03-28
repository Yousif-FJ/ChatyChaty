using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v1
{
    public class GetChatInfoResponse
    {
        public long ChatId { get; set; }
        public ProfileResponse Profile { get; set; }
    }
}
