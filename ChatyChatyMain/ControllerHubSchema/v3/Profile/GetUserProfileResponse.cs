using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public class GetUserProfileResponseBase
    {
        public long? ChatId { get; set; }
        public ProfileSchema Profile { get; set; }
    }
}
