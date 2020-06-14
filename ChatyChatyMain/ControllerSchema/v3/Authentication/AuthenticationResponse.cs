using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v3
{
    public class AuthenticationResponseBase
    {
        public string Token { get; set; }
        public ProfileSchema Profile { get; set; }
    }
}
