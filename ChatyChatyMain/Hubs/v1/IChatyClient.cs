using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v1
{
    public interface IChatClient
    {
        Task TestResponse(string message);
    }
}
