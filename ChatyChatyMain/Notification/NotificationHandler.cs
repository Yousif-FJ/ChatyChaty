using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Hubs.v3;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.NotficationServices.Handler
{
    /// <summary>
    /// Class that contain the common logic between notification handlers
    /// </summary>
    public abstract class NotificationHandlerCommon<T> : AsyncRequestHandler<T> where T : IRequest
    {
        protected readonly INotificationRepository notificationRepository;
        protected readonly IServiceProvider serviceProvider;
        public NotificationHandlerCommon(INotificationRepository notificationRepository,
            IServiceProvider serviceProvider)
        {
            this.notificationRepository = notificationRepository;
            this.serviceProvider = serviceProvider;
        }
    }
    public class NewMessageHandler : NotificationHandlerCommon<UserGotNewMessageAsync>
    {
        public NewMessageHandler(INotificationRepository notificationRepository, IServiceProvider serviceProvider)
            : base(notificationRepository, serviceProvider)
        {
        }

        protected override async Task Handle(UserGotNewMessageAsync request, CancellationToken cancellationToken)
        {
            var hubHelper = serviceProvider.GetService<HubHelper>();
            foreach (var item in request.userAndMessageId)
            {
                bool successful = await hubHelper.SendUpdateAsync(item.userId, item.messageId);
                if (successful == false)
                {
                    await notificationRepository.UserGotNewMessageAsync(item.userId);
                }
            }
        }
    }

    public class ChatUpdateHandler : NotificationHandlerCommon<UsersGotChatUpdateAsync>
    {
        public ChatUpdateHandler(INotificationRepository notificationRepository, IServiceProvider serviceProvider)
            : base(notificationRepository, serviceProvider)
        {
        }

        protected override async Task Handle(UsersGotChatUpdateAsync request, CancellationToken cancellationToken)
        {
            await notificationRepository.UsersGotChatUpdateAsync(
                  request.invokerAndReceiverIds.Select(m => m.ReceiverId).ToArray());
        }
    }

    public class MessageDeliveredHandler : NotificationHandlerCommon<UsersGotMessageDeliveredAsync>
    {
        public MessageDeliveredHandler(INotificationRepository notificationRepository, IServiceProvider serviceProvider)
            : base(notificationRepository, serviceProvider)
        {
        }

        protected override async Task Handle(UsersGotMessageDeliveredAsync request, CancellationToken cancellationToken)
        {
            await notificationRepository.UsersGotMessageDeliveredAsync(
                  request.userAndMessageIds.Select(m => m.userId).ToArray());
        }
    }
}

