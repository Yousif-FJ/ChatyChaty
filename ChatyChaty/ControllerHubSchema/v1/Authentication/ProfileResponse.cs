using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v1
{
    public record ProfileResponse(string Username, string DisplayName, string PhotoURL);
}
