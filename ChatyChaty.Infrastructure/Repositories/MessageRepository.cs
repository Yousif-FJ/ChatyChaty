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

        public async Task<Message> AddMessageAsync(Message Message)
        {
            var DBMessage = await dBContext.Messages.AddAsync(Message);
            await dBContext.SaveChangesAsync();
            return DBMessage.Entity;
        }

        public async Task<Message> GetMessageAsync(MessageId Id)
        {
            return await dBContext.Messages.FindAsync(Id);
        }

        public async Task<IEnumerable<Message>> GetMessagesForUserAsync(UserId userId)
        {
            return await dBContext.Messages
            .Where(m => m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId)
            .Include(m => m.Sender)
            .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetNewMessagesForUserAsync(MessageId messageId, UserId userId)
        {
            var message = await GetMessageAsync(messageId);

            return await dBContext.Messages.Where(
                m => m.TimeSent > message.TimeSent &&
                m.Conversation.FirstUserId == userId || m.Conversation.SecondUserId == userId
                ).Include(c => c.Sender).ToListAsync();
        }

        public async Task UpdateMessagesAsync(IEnumerable<Message> Messages)
        {
            dBContext.Messages.UpdateRange(Messages);
            await dBContext.SaveChangesAsync();
        }
    }
}
