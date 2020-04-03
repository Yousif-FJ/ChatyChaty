using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.NotficationHandler
{
    public interface INotificationHandler
    {
        Task<Notification> CheckForUpdates(long UserId);
        Task IntializeNofificationHandler(long UserId);
        Task UserGotChatUpdate(long UserId);
        Task UserGotNewMessage(long UserId);
        Task UserUpdatedProfile(long UserId);
        Task NotifySenderMessagesWhereDelivered(IEnumerable<long> SenderId);
    }
}
