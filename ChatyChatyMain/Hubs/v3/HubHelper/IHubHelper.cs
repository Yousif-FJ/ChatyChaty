using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    public interface IHubHelper
    {
        public Task<bool> SendUpdateAsync(long userId, long lastMessageId);
    }
}
