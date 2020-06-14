using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessagingModel
{
    public class ConversationInfo
    {
        public long ConversationId { get; set; }
        public string SecondUserDisplayName { get; set; }
        public string SecondUserUsername { get; set; }
        public long SecondUserId { get; set; }
        public string SecondUserPhoto { get; set; }
    }
}
