using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v1.Profile
{
    public class GetConversationInfoResponse
    {
        public long ConversationId { get; set; }
        public string PictureUrl { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
    }
}
