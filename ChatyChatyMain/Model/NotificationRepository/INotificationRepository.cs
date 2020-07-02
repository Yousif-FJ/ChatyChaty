using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.NotificationRepository
{
    /// <summary>
    /// Interface that encapsulate logic for accessing notification data from a source
    /// </summary>
    public interface INotificationRepository
    {
        Task<Notification> CheckForUpdatesAsync(long userId);
        Task<Notification> IntializeNotificationHandlerAsync(long userId);
        Task UsersGotChatUpdateAsync(params long[] userIds);
        Task UserGotNewMessageAsync(long userId);
        Task UsersGotMessageDeliveredAsync(params long[] userIds);
    }
}
