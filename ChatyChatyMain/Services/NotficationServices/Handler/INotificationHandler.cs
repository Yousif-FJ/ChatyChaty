using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.NotificationServices
{
    /// <summary>
    /// Interface that handles notification events
    /// </summary>
    public interface INotificationHandler
    {
        Task IntializeNotificationHandlerAsync(long userId);
        Task UsersGotChatUpdateAsync(params (long InvokerId, long ReceiverId)[] invokerAndReceiverIds);
        Task UserGotNewMessageAsync(params (long userId, long messageId)[] userAndMessageId);
        Task UsersGotMessageDeliveredAsync(params (long userId, long messageId)[] userAndMessageIds);
    }
}
