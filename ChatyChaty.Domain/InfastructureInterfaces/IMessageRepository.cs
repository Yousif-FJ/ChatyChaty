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
        Task<IEnumerable<Message>> GetMessagesFromConversationIdsAsync(long messageId, IEnumerable<long> conversationsIds);
        Task MarkAsReadAsync(IEnumerable<Message> messages);
        Task<Message> AddMessageAsync(Message message);
    }
}
