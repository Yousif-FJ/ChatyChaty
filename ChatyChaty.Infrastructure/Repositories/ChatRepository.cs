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

        public async Task<Conversation> GetConversationAsync(ConversationId ConversationId)
        {
            return await dBContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == ConversationId);
        }

        public async Task<IEnumerable<Conversation>> GetConversationsWithUsersAsync(UserId UserId)
        {
            return await dBContext.Conversations
                .Where(c => c.FirstUserId == UserId || c.SecondUserId == UserId)
                .Include(c => c.FirstUser).Include(c => c.SecondUser)
                .ToListAsync();
        }

        public async Task<Conversation> AddConversationAsync(Conversation conversation)
        {
            if (conversation is null)
            {
                throw new ArgumentNullException(nameof(conversation));
            }

            await dBContext.Conversations.AddAsync(conversation);
            await dBContext.SaveChangesAsync();
            return conversation;
        }


        public async Task<IEnumerable<Conversation>> GetConversationsAsync(UserId userId)
        {
            return await dBContext.Conversations
               .Where(c => c.FirstUserId == userId || c.SecondUserId == userId)
               .ToListAsync();
        }

        public async Task<Conversation> FindConversationAsync(UserId user1Id, UserId user2Id)
        {
            var conversation = await dBContext.Conversations.FirstOrDefaultAsync(
                c => (c.FirstUserId == user1Id && c.SecondUserId == user2Id) ||
                     (c.FirstUserId == user2Id && c.SecondUserId == user1Id)
                        );

            return conversation;
        }
    }
}
