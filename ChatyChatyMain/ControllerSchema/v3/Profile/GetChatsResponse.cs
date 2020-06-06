using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v3
{
    public class GetChatsResponse: ResponseBase
    {
        public new IEnumerable<GetUserProfileResponseBase> Data { get; set; }
    }
}
