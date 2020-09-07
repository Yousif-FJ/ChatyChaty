using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.NotificationRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.NotificationServices
{
    public class NotificationGetter : INotificationGetter
    {
        private readonly INotificationRepository notificationRepository;

        public NotificationGetter(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }
        public async Task<Notification> CheckForUpdatesAsync(long userId)
        {
            return await notificationRepository.CheckForUpdatesAsync(userId);
        }
    }
}
