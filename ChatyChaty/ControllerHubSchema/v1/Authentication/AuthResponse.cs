using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v1
{
    public record AuthResponse(string Token, ProfileResponse Profile);
}
