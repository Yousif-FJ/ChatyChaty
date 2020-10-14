using ChatyChaty.Model.DBModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.Repositories.ChatRepository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatyChatyContext dBContext;

        public ChatRepository(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
        }

        public async Task<Conversation> GetConversationAsync(long ConversationId)
        {
            return await dBContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == ConversationId);
        }

        public async Task<IEnumerable<long>> GetUserConversationIdsAsync(long UserId)
        {
            return await dBContext.Conversations
                .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
                .Select(c => c.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsWithUsersAsync(long UserId)
        {
            return await dBContext.Conversations
                .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
                .Include(c => c.FirstUser).Include(c => c.SecondUser)
                .ToListAsync();
        }


        public async Task<Conversation> GetConversationForUsersAsync(long User1Id, long User2Id)
        {
            var conversation = await dBContext.Conversations.FirstOrDefaultAsync(
                c => c.FirstUserId == User1Id && c.SecondUserId == User2Id
            );

            if (conversation == null)
            {
                conversation = await dBContext.Conversations.FirstOrDefaultAsync(
                    c => c.FirstUserId == User2Id && c.SecondUserId == User1Id
                );
            }
            //convestaion not found create new one
            if (conversation == null)
            {
                var newConversation = new Conversation()
                {
                    FirstUserId = User1Id,
                    SecondUserId = User2Id,
                };
                var resultConv = await dBContext.Conversations.AddAsync(newConversation);
                await dBContext.SaveChangesAsync();
                return resultConv.Entity;
            }
            else
            {
                return conversation;
            }
        }

        public async Task<IEnumerable<long>> GetUserContactIdsAsync(long userId)
        {
            var conversations = await dBContext.Conversations
               .Where(c => (c.FirstUserId == userId || c.SecondUserId == userId))
               .ToListAsync();

            List<long> userIdsList = new List<long>();
            foreach (var conversation in conversations)
            {
                if (conversation.FirstUserId == userId)
                {
                    userIdsList.Add(conversation.SecondUserId);
                }
                else if (conversation.SecondUserId == userId)
                {
                    userIdsList.Add(conversation.FirstUserId);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Conversatoin is not for the user");
                }
            }
            return userIdsList;
        }
    }
}
