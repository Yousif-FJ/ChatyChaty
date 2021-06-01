using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Infrastructure.Repositories.MessageRepository
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

        public async Task<Message> AddAsync(Message Message)
        {
            var DBMessage = await dBContext.Messages.AddAsync(Message);
            await dBContext.SaveChangesAsync();
            return DBMessage.Entity;
        }

        public Task<Message> GetAsync(MessageId Id)
        {
            return dBContext.Messages.Include(m => m.Sender).FirstOrDefaultAsync(m => m.Id == Id);
        }

        public Task<List<Message>> GetForChatAsync(ConversationId conversationId)
        {
            return dBContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .Include(m => m.Sender)
                .AsSplitQuery()
                .ToListAsync();
        }

        public  Task<List<Message>> GetAllAsync(UserId userId)
        {
            return dBContext.Messages
            .Where(m => m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId)
            .Include(m => m.Sender)
            .AsSplitQuery()
            .ToListAsync();
        }

        public async Task<List<Message>> GetNewAsync(MessageId messageId, UserId userId)
        {
            var message = await GetAsync(messageId);

            return await dBContext.Messages.Where(
                m => m.SentTime > message.SentTime &&
                m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId
                ).Include(c => c.Sender)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Message> Messages)
        {
            dBContext.Messages.UpdateRange(Messages);
            await dBContext.SaveChangesAsync();
        }

        public Task RemoveOverLimit(UserId userId,int numberOfMessageToRemove = 100)
        {
            var messages = dBContext.Messages
            .Where(m => m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId)
            .Include(m => m.Sender)
            .TakeLast(numberOfMessageToRemove)
            .AsSplitQuery();

            dBContext.Messages.RemoveRange(messages);

            return dBContext.SaveChangesAsync();
        }
    }
}
