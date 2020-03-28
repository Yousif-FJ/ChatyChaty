using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v1
{
    public class ProfileResponse
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string PhotoURL { get; set; }
    }
}
