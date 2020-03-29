using ChatyChaty.Model.DBModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessageRepository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatyChatyContext dBContext;

        public MessageRepository(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
        }

        public async Task<Message> AddMessage(Message Message)
        {
            var DBMessage = await dBContext.Messages.AddAsync(Message);
            await dBContext.SaveChangesAsync();
            return DBMessage.Entity;
        }

        public async Task<Conversation> CreateConversationForUsers(long User1Id, long User2Id)
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

        public async Task<Conversation> FindConversationForUsers(long User1Id, long User2Id)
        {
            var conversation = await dBContext.Conversations.FirstOrDefaultAsync(
                c => c.FirstUserId == User1Id || c.SecondUserId == User1Id
            );

            if (conversation == null)
            {
                conversation = await dBContext.Conversations.FirstOrDefaultAsync(
                    c => c.FirstUserId == User2Id || c.SecondUserId == User2Id
                );
            }
            return conversation;
        }

        public async Task<Conversation> GetConversation(long ConversationId)
        {
            return await dBContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == ConversationId);
        }

        public async Task<Message> GetMessage(long Id)
        {
            return await dBContext.Messages.FindAsync(Id);
        }

        public async Task<IEnumerable<Message>> GetMessagesFromConversationIds(long MessageId, IQueryable<long> ConversationsIds)
        {
            return await dBContext.Messages.Where(
                m => m.Id > MessageId &&
                ConversationsIds.Any(id => id == m.ConversationId)
                ).Include(c => c.Sender).ToListAsync();
        }

        public async Task<AppUser> GetUser(long Id)
        {
            return await dBContext.Users.FindAsync(Id);
        }

        public IQueryable<long> GetUserConversationIds(long UserId)
        {
            return dBContext.Conversations
                .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
                .Select(c => c.Id);
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsWithUsers(long UserId)
        {
            return await dBContext.Conversations
                .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
                .Include(c => c.FirstUser).Include(c => c.SecondUser)
                .ToListAsync();
        }

        public async Task<bool> IsConversationForUser(long ConversationId, long UserId)
        {
            return await dBContext.Conversations
                .Include(c => c.Messages)
                .AnyAsync(c => c.Id == ConversationId);
        }

        public async Task<bool> IsThereNewMessageInConversationIds(long MessageId, IQueryable<long> ConversationsIds)
        {
            return await dBContext.Messages.AnyAsync(
                 m => m.Id > MessageId &&
                ConversationsIds.Any(id => id == m.ConversationId)
                 );
        }

        public async Task MarkAsRead(IEnumerable<Message> Messages)
        {
            foreach (var Message in Messages)
            {
                Message.Delivered = true;
            }
            dBContext.Messages.UpdateRange(Messages);
            await dBContext.SaveChangesAsync();
        }

        public async Task<string> UpdateDisplayName(long UserId, string NewDisplayName)
        {
            var User = await dBContext.Users.FindAsync(UserId);
            User.DisplayName = NewDisplayName;
            var Updated = dBContext.Users.Update(User);
            await dBContext.SaveChangesAsync();
            return Updated.Entity.DisplayName;

        }
    }
}
