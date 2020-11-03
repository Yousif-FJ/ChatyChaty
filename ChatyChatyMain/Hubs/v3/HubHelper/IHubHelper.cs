using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    public interface IHubHelper
    {
        public Task<bool> SendMessageUpdateAsync(long userId, long messageId);
        public Task<bool> SendChatUpdateAsync(long receiverId, long chatId) ;
        public Task<bool> SendMessageStatusUpdateAsync(long userId, long messageId);
    }
}
