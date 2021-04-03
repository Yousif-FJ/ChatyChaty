using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v1
{
    public class UserProfileResponse
    {
        public string ChatId { get; set; }
        public ProfileResponse Profile { get; set; }
    }
}
