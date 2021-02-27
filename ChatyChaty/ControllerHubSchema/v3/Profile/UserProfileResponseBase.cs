using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public class UserProfileResponseBase
    {
        public string ChatId { get; set; }
        public ProfileResponseBase Profile { get; set; }
    }
}
