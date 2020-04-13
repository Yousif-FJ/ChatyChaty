using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v1
{
    public class GetUserProfileResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public long? ChatId { get; set; }
        public ProfileResponse Profile { get; set; }
    }
}
