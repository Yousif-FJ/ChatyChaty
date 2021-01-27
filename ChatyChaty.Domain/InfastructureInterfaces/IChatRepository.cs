using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    public interface IChatRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsAsync(long userId);
        Task<IEnumerable<Conversation>> GetConversationsWithUsersAsync(long userId);
        Task<Conversation> GetConversationAsync(long Id);
        Task<Conversation> CreateConversationAsync(long user1Id, long user2Id);
    }
}
