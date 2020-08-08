using ChatyChaty.Hubs.v1;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.NotificationRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider serviceProvider;

        public NotificationHandler(INotificationRepository notificationRepository,
            IServiceProvider serviceProvider)
        {
            this.notificationRepository = notificationRepository;
            this.serviceProvider = serviceProvider;
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
            var hubHelper = serviceProvider.GetService<HubHelper>();
            bool successful = await hubHelper.SendUpdate(userId);
            if (successful == false)
            {
                await notificationRepository.UserGotNewMessageAsync(userId);
            }
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
