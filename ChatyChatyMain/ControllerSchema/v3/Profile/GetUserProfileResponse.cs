using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v3
{
    public class GetUserProfileResponse : ResponseBase
    {
        public new GetUserProfileResponseBase Data { get; set; }
    }

    public class GetUserProfileResponseBase
    {
        public long? ChatId { get; set; }
        public ProfileSchema Profile { get; set; }
    }
}
