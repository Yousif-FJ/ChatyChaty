using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    public interface IChatRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsAsync(UserId userId);
        Task<IEnumerable<Conversation>> GetConversationsWithUsersAsync(UserId userId);
        Task<Conversation> GetConversationAsync(ConversationId Id);
        Task<Conversation> AddConversationAsync(Conversation conversation);
        Task<Conversation> FindConversationAsync(UserId user1Id, UserId user2Id);
    }
}
