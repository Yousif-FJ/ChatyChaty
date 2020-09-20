using ChatyChaty.Model.DBModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.Repositories.MessageRepository
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

        public async Task<Message> GetMessageAsync(long Id)
        {
            return await dBContext.Messages.FindAsync(Id);
        }

        public async Task<IEnumerable<Message>> GetMessagesFromConversationIdsAsync(long MessageId, IEnumerable<long> ConversationsIds)
        {
            return await dBContext.Messages.Where(
                m => m.Id > MessageId &&
                ConversationsIds.Any(id => id == m.ConversationId)
                ).Include(c => c.Sender).ToListAsync();
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
    }
}
