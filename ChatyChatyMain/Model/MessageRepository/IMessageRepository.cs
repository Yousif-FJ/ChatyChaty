using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessageRepository
{
    public interface IMessageRepository
    {
        Task<AppUser> GetUserAsync(long Id);
        Task<Conversation> GetConversationAsync(long Id);
        Task<Message> GetMessageAsync(long Id);
        Task<bool> IsConversationForUserAsync(long ConversationId, long UserId);
        Task<Conversation> FindConversationForUsersAsync(long User1Id, long User2Id);
        Task<Conversation> CreateConversationForUsersAsync(long User1Id, long User2Id);
        IQueryable<long> GetUserConversationIds(long UserId);
        Task<IEnumerable<Conversation>> GetUserConversationsWithUsersAsync(long UserId);
        Task<IEnumerable<Message>> GetMessagesFromConversationIdsAsync(long MessageId, IQueryable<long> ConversationsIds);
        Task MarkAsReadAsync(IEnumerable<Message> Messages);
        Task<Message> AddMessageAsync(Message Message);
        Task<bool> IsThereNewMessageInConversationIdsAsync(long MessageId, IQueryable<long> ConversationsIds);
        Task<string> UpdateDisplayNameAsync(long UserId, string NewDisplayName);
    }
}
