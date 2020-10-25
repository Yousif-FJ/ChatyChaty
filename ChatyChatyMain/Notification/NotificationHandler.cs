using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Hubs.v3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.NotficationServices.Handler
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

        public async Task UserGotNewMessageAsync(params (long userId, long messageId)[] userAndMessageId)
        {
            var hubHelper = serviceProvider.GetService<HubHelper>();
            foreach (var item in userAndMessageId)
            {
                bool successful = await hubHelper.SendUpdateAsync(item.userId, item.messageId);
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
