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

        public async Task<Message> GetAsync(MessageId Id)
        {
            var message = await dBContext.Messages.Include(m => m.Sender).FirstOrDefaultAsync(m => m.Id == Id);
            message.SetSender(message.Sender.UserName);
            return message;
        }

        public Task<List<Message>> GetForChatAsync(ConversationId conversationId)
        {
            return dBContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .Select(m => m.SetSender(m.Sender.UserName))
                .AsSplitQuery()
                .ToListAsync();
        }

        public  Task<List<Message>> GetAllAsync(UserId userId)
        {
            return dBContext.Messages
            .Where(m => m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId)
            .Select(m => m.SetSender(m.Sender.UserName))
            .AsSplitQuery()
            .ToListAsync();
        }

        public async Task<List<Message>> GetNewAsync(DateTime dateTime, UserId userId)
        {
            return await dBContext.Messages.Where(
                m => (m.SentTime > dateTime &&
                (m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId)))
                .Select(m => m.SetSender(m.Sender.UserName))
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
            .TakeLast(numberOfMessageToRemove);

            dBContext.Messages.RemoveRange(messages);

            return dBContext.SaveChangesAsync();
        }
    }
}
