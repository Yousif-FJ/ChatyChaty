using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.HttpSchemas.Authentication
{
    public record ProfileResponse(string Username, string DisplayName, string PhotoURL);
}
