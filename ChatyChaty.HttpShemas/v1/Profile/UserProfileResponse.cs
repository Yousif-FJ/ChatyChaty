using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Profile
{
    public record UserProfileResponse(string ChatId, ProfileResponse Profile);
}
