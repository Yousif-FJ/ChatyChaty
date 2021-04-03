using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Model.MessagingModel;
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
        Task<SendMessageModel> SendMessage(ConversationId conversationId, UserId senderId, string messageBody);
        Task<GetMessagesModel> GetNewMessages(UserId userId, MessageId lastMessageId);
        Task<GetMessagesModel> GetMessageForChat(UserId userId, ConversationId conversationId);
        Task<IsDeliveredModel> IsDelivered(UserId userId, MessageId messageId);
    }
}
