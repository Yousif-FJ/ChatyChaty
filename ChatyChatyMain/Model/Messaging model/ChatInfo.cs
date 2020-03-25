using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.Messaging_model
{
    public class ChatInfo
    {
        public long ChatId { get; set; }
        public string SecondUserDisplayName { get; set; }
        public string SecondUserUsername { get; set; }
        public long SecondUserId { get; set; }
    }
}
