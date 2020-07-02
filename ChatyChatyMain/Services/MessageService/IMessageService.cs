using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessagingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    /// <summary>
    /// Interface that handle the messaging logic
    /// </summary>
    public interface IMessageService
    {
        Task<long> NewConversation(long senderId, long receiverId);
        Task<SendMessageModel> SendMessage(long conversationId, long senderId, string messageBody);
        Task<GetNewMessagesModel> GetNewMessages(long userId, long lastMessageId);
        Task<bool?> CheckForNewMessages(long userId, long lastMessageId);
        Task<IsDeliveredModel> IsDelivered(long userId, long messageId);
        Task<IEnumerable<ConversationInfo>> GetConversations(long userId);
    }
}
