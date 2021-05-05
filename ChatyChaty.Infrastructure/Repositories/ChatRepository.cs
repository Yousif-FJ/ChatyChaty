using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Infrastructure.Repositories.ChatRepository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatyChatyContext dBContext;

        public ChatRepository(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
        }

        public Task<Conversation> GetAsync(ConversationId ConversationId)
        {
            return dBContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == ConversationId);
        }

        public Task<List<Conversation>> GetWithUsersAsync(UserId UserId)
        {
            return dBContext.Conversations
                .Where(c => c.FirstUserId == UserId || c.SecondUserId == UserId)
                .Include(c => c.FirstUser).Include(c => c.SecondUser)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Conversation> AddAsync(Conversation conversation)
        {
            if (conversation is null)
            {
                throw new ArgumentNullException(nameof(conversation));
            }

            await dBContext.Conversations.AddAsync(conversation);
            await dBContext.SaveChangesAsync();
            return conversation;
        }


        public Task<List<Conversation>> GetAllAsync(UserId userId)
        {
            return dBContext.Conversations
               .Where(c => c.FirstUserId == userId || c.SecondUserId == userId)
               .ToListAsync();
        }

        public Task<Conversation> GetAsync(UserId user1Id, UserId user2Id)
        {
            var conversation = dBContext.Conversations.FirstOrDefaultAsync(
                c => (c.FirstUserId == user1Id && c.SecondUserId == user2Id) ||
                     (c.FirstUserId == user2Id && c.SecondUserId == user1Id)
                        );

            return conversation;
        }
    }
}
