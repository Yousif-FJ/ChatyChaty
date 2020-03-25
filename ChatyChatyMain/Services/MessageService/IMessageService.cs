using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.Messaging_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public interface IMessageService
    {
        Task<long> NewChat(long SenderId, long ReceiverId);
        Task<Message> SendMessage(long ChatId, long SenderId, string MessageBody);
        Task<IEnumerable<Message>> GetNewMessages(long UserId, long LastMessageId);
        Task<bool?> CheckForNewMessages(long UserId, long LastMessageId);
        Task<bool?> IsDelivered(long UserId, long MessageId);
        Task<ChatInfo> GetChatInfo(long UserId, long ChatId);
    }
}
