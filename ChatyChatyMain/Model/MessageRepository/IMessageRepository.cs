using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessageRepository
{
    public interface IMessageRepository
    {
        Task<AppUser> GetUser(long Id);
        Task<Conversation> GetConversation(long Id);
        Task<Message> GetMessage(long Id);
        Task<bool> IsConversationForUser(long ConversationId, long UserId);
        Task<Conversation> FindConversationForUsers(long User1Id, long User2Id);
        Task<Conversation> CreateConversationForUsers(long User1Id, long User2Id);
        IQueryable<long> GetUserConversationIds(long UserId);
        Task<IEnumerable<Conversation>> GetUserConversationsWithUsers(long UserId);
        Task<IEnumerable<Message>> GetMessagesFromConversationIds(long MessageId, IQueryable<long> ConversationsIds);
        Task MarkAsRead(IEnumerable<Message> Messages);
        Task<Message> AddMessage(Message Message);
        Task<bool> IsThereNewMessageInConversationIds(long MessageId, IQueryable<long> ConversationsIds);
        Task<string> UpdateDisplayName(long UserId, string NewDisplayName);
    }
}
