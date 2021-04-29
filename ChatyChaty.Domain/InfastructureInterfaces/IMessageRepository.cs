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
        Task<Message> GetAsync(MessageId Id);
        Task<IEnumerable<Message>> GetAllAsync(UserId userId);
        Task<IEnumerable<Message>> GetNewAsync(MessageId messageId, UserId userId);
        Task<IList<Message>> GetForChatAsync(ConversationId conversationId, int count = 100);
        Task UpdateRangeAsync(IEnumerable<Message> messages);
        Task<Message> AddAsync(Message message);
    }
}
