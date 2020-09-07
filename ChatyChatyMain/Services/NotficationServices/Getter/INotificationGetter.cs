using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.NotificationServices
{
    public interface INotificationGetter
    {
        Task<Notification> CheckForUpdatesAsync(long userId);
    }
}
