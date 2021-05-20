using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Entities
{
    public record AuthenticationResult(bool IsSuccessful, string Error);
}
