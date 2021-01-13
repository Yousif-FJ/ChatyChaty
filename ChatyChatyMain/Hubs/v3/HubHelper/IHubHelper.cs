using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    public interface IHubHelper
    {
        public Task<bool> TrySendMessageUpdateAsync(long userId, long messageId);
        public Task<bool> TrySendChatUpdateAsync(long receiverId, long chatId) ;
        public Task<bool> TrySendMessageStatusUpdateAsync(long userId, long messageId);
    }
}
