using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public interface IMessageService
    {
        Task<Message> SendNewMessage(long SenderId, long ReceiverId, string MessageBody);
        Task<Message> SendMessage(long ConversationId, long SenderId, string MessageBody);
        Task<IEnumerable<Message>> GetNewMessages(long UserId, long LastMessageID);
        Task<bool?> IsDelivered(long UserId, long MessageId);
    }
}
