using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.NotficationServices.Getter
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
            return await notificationRepository.GetNotificationAsync(userId);
        }
    }
}
