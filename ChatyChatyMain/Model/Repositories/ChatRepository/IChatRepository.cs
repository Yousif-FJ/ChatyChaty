using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.Repositories.ChatRepository
{
    public interface IChatRepository
    {
        Task<bool> IsConversationForUserAsync(long conversationId, long userId);
        Task<Conversation> FindConversationForUsersAsync(long user1Id, long user2Id);
        Task<Conversation> CreateConversationForUsersAsync(long user1Id, long user2Id);
        Task<IEnumerable<long>> GetUserContactIdsAsync(long userId);
        Task<IEnumerable<long>> GetUserConversationIdsAsync(long userId);
        Task<IEnumerable<Conversation>> GetUserConversationsWithUsersAsync(long userId);
        Task<Conversation> GetConversationAsync(long Id);
    }
}
