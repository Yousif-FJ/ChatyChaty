using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    public interface IChatRepository
    {
        Task<List<Conversation>> GetAllAsync(UserId userId);
        Task<List<Conversation>> GetWithUsersAsync(UserId userId);
        Task<Conversation> GetAsync(ConversationId Id);
        Task<Conversation> GetAsync(UserId user1Id, UserId user2Id);
        Task<Conversation> AddAsync(Conversation conversation);
    }
}
