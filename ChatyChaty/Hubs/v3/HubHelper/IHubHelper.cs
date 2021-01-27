using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    public interface IHubHelper
    {
        public bool TrySendMessageUpdate(long receiverId, IEnumerable<Message> messages);
        public bool TrySendChatUpdate(long receiverId, ProfileAccountModel chatInfo);
        public bool TrySendMessageStatusUpdate(long userId, long chatId, long messageId);
    }
}
