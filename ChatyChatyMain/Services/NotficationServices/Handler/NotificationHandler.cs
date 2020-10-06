using ChatyChaty.Hubs.v3;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.NotificationRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.NotificationServices
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

        public async Task IntializeNotificationHandlerAsync(long userId)
        {
            await notificationRepository.IntializeNotificationHandlerAsync(userId);
        }

        public async Task UserGotNewMessageAsync(params (long userId, long messageId)[] userAndMessageId)
        {
            var hubHelper = serviceProvider.GetService<HubHelper>();
            foreach (var item in userAndMessageId)
            {
                bool successful = await hubHelper.SendUpdateAsync(item.userId);
                if (successful == false)
                {
                    await notificationRepository.UserGotNewMessageAsync(item.userId);
                }
            }
        }

        public async Task UsersGotChatUpdateAsync(params (long InvokerId, long ReceiverId)[] invokerAndReceiverIds)
        {
            await notificationRepository.UsersGotChatUpdateAsync(
                invokerAndReceiverIds.Select(m => m.ReceiverId).ToArray());
        }

        public async Task UsersGotMessageDeliveredAsync(params (long userId, long messageId)[] userAndMessageIds)
        {
            await notificationRepository.UsersGotMessageDeliveredAsync(
                userAndMessageIds.Select(m => m.userId).ToArray());
        }
    }
}
