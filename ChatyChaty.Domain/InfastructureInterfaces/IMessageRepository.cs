using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    /// <summary>
    /// Interface that encapsulate the logic of accessing messages from a source
    /// </summary>
    public interface IMessageRepository
    {
        Task<Message> GetMessageAsync(long Id);
        Task<IEnumerable<Message>> GetLastDeliveredMessageForEachChat(long userId);
        Task<IEnumerable<Message>> GetMessagesForUser(long messageId, long userId);
        Task UpdateMessagesAsync(IEnumerable<Message> messages);
        Task<Message> AddMessageAsync(Message message);
    }
}
