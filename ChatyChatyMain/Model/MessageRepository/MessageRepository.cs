using ChatyChaty.Model.DBModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessageRepository
{
    /// <summary>
    /// class that encapsulate the logic of accessing messages from EntityFramework managed DB
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatyChatyContext dBContext;

        public MessageRepository(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
        }

        public async Task<Message> AddMessageAsync(Message Message)
        {
            var DBMessage = await dBContext.Messages.AddAsync(Message);
            await dBContext.SaveChangesAsync();
            return DBMessage.Entity;
        }

        public async Task<Conversation> CreateConversationForUsersAsync(long User1Id, long User2Id)
        {
            var conversation = new Conversation()
            {
                FirstUserId = User1Id,
                SecondUserId = User2Id,
            };
            var resultConv = await dBContext.Conversations.AddAsync(conversation);
            await dBContext.SaveChangesAsync();
            return resultConv.Entity;
        }

        public async Task<Conversation> FindConversationForUsersAsync(long User1Id, long User2Id)
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
            return conversation;
        }

        public async Task<Conversation> GetConversationAsync(long ConversationId)
        {
            return await dBContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == ConversationId);
        }

        public async Task<Message> GetMessageAsync(long Id)
        {
            return await dBContext.Messages.FindAsync(Id);
        }

        public async Task<IEnumerable<Message>> GetMessagesFromConversationIdsAsync(long MessageId, IQueryable<long> ConversationsIds)
        {
            return await dBContext.Messages.Where(
                m => m.Id > MessageId &&
                ConversationsIds.Any(id => id == m.ConversationId)
                ).Include(c => c.Sender).ToListAsync();
        }

        public async Task<AppUser> GetUserAsync(long Id)
        {
            return await dBContext.Users.FindAsync(Id);
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

        public IQueryable<long> GetUserConversationIdsAsync(long UserId)
        {
            return dBContext.Conversations
                .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
                .Select(c => c.Id);
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsWithUsersAsync(long UserId)
        {
            return await dBContext.Conversations
                .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
                .Include(c => c.FirstUser).Include(c => c.SecondUser)
                .ToListAsync();
        }

        public async Task<bool> IsConversationForUserAsync(long ConversationId, long UserId)
        {
            return await dBContext.Conversations
                .Include(c => c.Messages)
                .AnyAsync(c => c.Id == ConversationId);
        }

        public async Task<bool> IsThereNewMessageInConversationIdsAsync(long MessageId, IQueryable<long> ConversationsIds)
        {
            return await dBContext.Messages.AnyAsync(
                 m => m.Id > MessageId &&
                ConversationsIds.Any(id => id == m.ConversationId)
                 );
        }

        public async Task MarkAsReadAsync(IEnumerable<Message> Messages)
        {
            foreach (var Message in Messages)
            {
                Message.Delivered = true;
            }
            dBContext.Messages.UpdateRange(Messages);
            await dBContext.SaveChangesAsync();
        }

        public async Task<string> UpdateDisplayNameAsync(long UserId, string NewDisplayName)
        {
            var User = await dBContext.Users.FindAsync(UserId);
            User.DisplayName = NewDisplayName;
            var Updated = dBContext.Users.Update(User);
            await dBContext.SaveChangesAsync();
            return Updated.Entity.DisplayName;

        }
    }
}
