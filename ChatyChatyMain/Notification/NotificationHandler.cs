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
        protected readonly IHubHelper hubHelper;

        public NotificationHandlerCommon(INotificationRepository notificationRepository, IHubHelper hubHelper)
        {
            this.notificationRepository = notificationRepository;
            this.hubHelper = hubHelper;
        }
    }
    public class NewMessageHandler : NotificationHandlerCommon<UserGotNewMessageAsync>
    {
        public NewMessageHandler(INotificationRepository notificationRepository, IHubHelper hubHelper) : base(notificationRepository, hubHelper)
        {
        }

        protected override async Task Handle(UserGotNewMessageAsync request, CancellationToken cancellationToken)
        {
            foreach (var (userId, messageId) in request.userAndMessageId)
            {
                bool successful = await hubHelper.SendUpdateAsync(userId, messageId);
                if (successful == false)
                {
                    await notificationRepository.UserGotNewMessageAsync(userId);
                }
            }
        }
    }

    public class ChatUpdateHandler : NotificationHandlerCommon<UsersGotChatUpdateAsync>
    {
        public ChatUpdateHandler(INotificationRepository notificationRepository, IHubHelper hubHelper) : base(notificationRepository, hubHelper)
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
        public MessageDeliveredHandler(INotificationRepository notificationRepository, IHubHelper hubHelper) : base(notificationRepository, hubHelper)
        {
        }

        protected override async Task Handle(UsersGotMessageDeliveredAsync request, CancellationToken cancellationToken)
        {
            await notificationRepository.UsersGotMessageDeliveredAsync(
                  request.userAndMessageIds.Select(m => m.userId).ToArray());
        }
    }
}

