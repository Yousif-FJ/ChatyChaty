using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Model.MessagingModel;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.Hubs.v3;
using MediatR;
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
        private readonly IMessageRepository messageRepository;

        public NewMessageHandler(INotificationRepository notificationRepository, IHubHelper hubHelper, IMessageRepository messageRepository) : base(notificationRepository, hubHelper)
        {
            this.messageRepository = messageRepository;
        }

        protected override async Task Handle(UserGotNewMessageAsync request, CancellationToken cancellationToken)
        {
            foreach (var (userId, messageId) in request.UserAndMessageId)
            {

                var message = await messageRepository.GetMessageAsync(messageId);

                bool successful = hubHelper.TrySendMessageUpdate(userId, new List<Message> { message});
                if (successful == false)
                {
                    await notificationRepository.StoreUserNewMessageStatusAsync(userId);
                }
            }
        }
    }

    public class ChatUpdateHandler : NotificationHandlerCommon<UsersGotChatUpdateAsync>
    {
        private readonly IAccountManager accountManager;

        public ChatUpdateHandler(INotificationRepository notificationRepository, IHubHelper hubHelper, IAccountManager accountManager) : base(notificationRepository, hubHelper)
        {
            this.accountManager = accountManager;
        }

        protected override async Task Handle(UsersGotChatUpdateAsync request, CancellationToken cancellationToken)
        {
            foreach (var (receiverId, chatId) in request.InvokerAndReceiverIds)
            {
                var chat = await accountManager.GetConversation(chatId, receiverId);
                bool successful = hubHelper.TrySendChatUpdate(receiverId,chat);
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
            foreach (var (receieverId, chatId, messageId) in request.MessageInfo)
            {
                var sentSuccessfully = hubHelper.TrySendMessageStatusUpdate(receieverId, chatId, messageId);
                if (sentSuccessfully == false)
                {
                    await notificationRepository.StoreUsersMessageStatusAsync(receieverId);
                }
            }
        }
    }
    
    public class ProfileUpdateHandler : NotificationHandlerCommon<UserUpdatedTheirProfileAsync>
    {
        private readonly IAccountManager accountManager;

        public ProfileUpdateHandler(INotificationRepository notificationRepository, IHubHelper hubHelper, IAccountManager accountManager) : base(notificationRepository, hubHelper)
        {
            this.accountManager = accountManager;
        }

        protected async override Task Handle(UserUpdatedTheirProfileAsync request, CancellationToken cancellationToken)
        {
            IEnumerable<ProfileAccountModel> chats = await accountManager.GetConversations(request.UserId);
            foreach (var chat in chats)
            {
                var successful = hubHelper.TrySendChatUpdate(chat.UserId.Value,chat);
                if (successful == false)
                {
                    await notificationRepository.StoreUsersChatUpdateStatusAsync(chat.UserId.Value);
                }
            }
        }
    }
    
}

