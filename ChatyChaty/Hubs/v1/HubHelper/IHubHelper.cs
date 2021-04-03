using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v1
{
    public interface IHubHelper
    {
        public bool TrySendMessageUpdate(UserId receiverId, IEnumerable<Message> messages);
        public bool TrySendChatUpdate(UserId receiverId, ProfileAccountModel chatInfo);
        public bool TrySendMessageStatusUpdate(UserId userId, ConversationId chatId, MessageId messageId);
    }
}
