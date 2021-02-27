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
        Task<Message> GetMessageAsync(MessageId Id);
        Task<IEnumerable<Message>> GetLastDeliveredMessageForEachChat(UserId userId);
        Task<IEnumerable<Message>> GetMessagesForUser(UserId userId);
        Task<IEnumerable<Message>> GetNewMessagesForUser(MessageId messageId, UserId userId);
        Task UpdateMessagesAsync(IEnumerable<Message> messages);
        Task<Message> AddMessageAsync(Message message);
    }
}
