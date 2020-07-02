using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.NotificationRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    /// <summary>
    /// Class that handles notification events logic
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        private readonly INotificationRepository notificationRepository;

        public NotificationHandler(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        public async Task<Notification> CheckForUpdatesAsync(long userId)
        {
            return await notificationRepository.CheckForUpdatesAsync(userId);
        }

        public async Task IntializeNotificationHandlerAsync(long userId)
        {
            await notificationRepository.IntializeNotificationHandlerAsync(userId);
        }

        public async Task UserGotNewMessageAsync(long userId)
        {
            await notificationRepository.UserGotNewMessageAsync(userId);
        }

        public async Task UsersGotChatUpdateAsync(params long[] userIds)
        {
            await notificationRepository.UsersGotChatUpdateAsync(userIds);
        }

        public async Task UsersGotMessageDeliveredAsync(params long[] userIds)
        {
            await notificationRepository.UsersGotMessageDeliveredAsync(userIds);
        }
    }
}
