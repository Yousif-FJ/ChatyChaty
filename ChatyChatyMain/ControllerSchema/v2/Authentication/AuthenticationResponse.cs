using ChatyChaty.ControllerSchema.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v2
{
    public class AuthenticationResponse 
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public ProfileSchema Profile { get; set; }
    }
}
