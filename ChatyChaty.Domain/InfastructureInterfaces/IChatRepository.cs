using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    public interface IChatRepository
    {
        Task<Conversation> GetConversationForUsersAsync(long user1Id, long user2Id);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(long userId);
        Task<IEnumerable<Conversation>> GetUserConversationsWithUsersAsync(long userId);
        Task<Conversation> GetConversationAsync(long Id);
    }
}
