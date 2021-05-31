using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.MessageServices
{
    /// <summary>
    /// Interface that handle the messaging logic
    /// </summary>
    public interface IMessageService
    {
        Task<Message> SendMessage(ConversationId conversationId, UserId senderId, string messageBody);
        Task<IList<Message>> GetNewMessages(UserId userId, MessageId lastMessageId);
        Task<IList<Message>> GetMessageForChat(UserId userId, ConversationId conversationId);
        Task<bool> IsDelivered(UserId userId, MessageId messageId);
    }
}
