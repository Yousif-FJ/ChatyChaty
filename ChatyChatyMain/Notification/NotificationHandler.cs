using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
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
                bool successful = await hubHelper.TrySendMessageUpdateAsync(userId, messageId);
                if (successful == false)
                {
                    await notificationRepository.StoreUserNewMessageStatusAsync(userId);
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
            foreach (var (receiverId, chatId) in request.invokerAndReceiverIds)
            {
                bool successful = await hubHelper.TrySendChatUpdateAsync(receiverId,chatId);
                if (successful == false)
                {
                    await notificationRepository.StoreUsersChatUpdateStatusAsync(receiverId);
                }
            }
        }
    }

    public class MessageStatusHandler : NotificationHandlerCommon<UsersGotMessageStatusUpdateAsync>
    {
        public MessageStatusHandler(INotificationRepository notificationRepository, IHubHelper hubHelper) : base(notificationRepository, hubHelper)
        {
        }

        protected override async Task Handle(UsersGotMessageStatusUpdateAsync request, CancellationToken cancellationToken)
        {
            foreach (var (userId, messageId) in request.userAndMessageIds)
            {
                var sentSuccessfully = await hubHelper.TrySendMessageStatusUpdateAsync(userId,messageId);
                if (sentSuccessfully == false)
                {
                    await notificationRepository.StoreUsersMessageStatusAsync(userId);
                }
            }

        }
    }
    
    public class ProfileUpdateHandler : NotificationHandlerCommon<UserUpdatedTheirProfileAsync>
    {
        private readonly IChatRepository chatRepository;

        public ProfileUpdateHandler(INotificationRepository notificationRepository, IHubHelper hubHelper, IChatRepository chatRepository) : base(notificationRepository, hubHelper)
        {
            this.chatRepository = chatRepository;
        }

        protected async override Task Handle(UserUpdatedTheirProfileAsync request, CancellationToken cancellationToken)
        {
            IEnumerable<Conversation> chats = await chatRepository.GetConversationsAsync(request.UserId);
            foreach (var chat in chats)
            {
                var receiverId = chat.FindReceiverId(request.UserId);
                var successful = await hubHelper.TrySendChatUpdateAsync(receiverId, chat.Id);
                if (successful == false)
                {
                    await notificationRepository.StoreUsersChatUpdateStatusAsync(receiverId);
                }
            }
        }
    }
    
}

