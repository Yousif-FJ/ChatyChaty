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
        public Task<bool> TrySendMessageUpdate(UserId receiverId, IEnumerable<Message> messages);
        public Task<bool> TrySendChatUpdate(UserId receiverId, ProfileAccountModel chatInfo);
        public Task<bool> TrySendMessageStatusUpdate(UserId userId, ConversationId chatId, MessageId messageId);
    }
}
