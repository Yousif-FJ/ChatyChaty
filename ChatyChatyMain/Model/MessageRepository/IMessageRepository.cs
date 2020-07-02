using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessageRepository
{
    /// <summary>
    /// Interface that encapsulate the logic of accessing messages from a source
    /// </summary>
    public interface IMessageRepository
    {
        Task<AppUser> GetUserAsync(long Id);
        Task<Conversation> GetConversationAsync(long Id);
        Task<Message> GetMessageAsync(long Id);
        Task<bool> IsConversationForUserAsync(long conversationId, long userId);
        Task<Conversation> FindConversationForUsersAsync(long user1Id, long user2Id);
        Task<Conversation> CreateConversationForUsersAsync(long user1Id, long user2Id);
        Task<IEnumerable<long>> GetUserContactIdsAsync(long userId);
        IQueryable<long> GetUserConversationIdsAsync(long userId);
        Task<IEnumerable<Conversation>> GetUserConversationsWithUsersAsync(long userId);
        Task<IEnumerable<Message>> GetMessagesFromConversationIdsAsync(long messageId, IQueryable<long> conversationsIds);
        Task MarkAsReadAsync(IEnumerable<Message> messages);
        Task<Message> AddMessageAsync(Message message);
        Task<bool> IsThereNewMessageInConversationIdsAsync(long messageId, IQueryable<long> conversationsIds);
        Task<string> UpdateDisplayNameAsync(long userId, string newDisplayName);
    }
}
