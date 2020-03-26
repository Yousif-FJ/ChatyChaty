using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v1.Profile
{
    public class GetChatInfoResponse
    {
        public long ChatId { get; set; }
        public string PhotoURL { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
    }
}
