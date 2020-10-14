using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessagingModel;
using ChatyChaty.Model.Repositories.ChatRepository;
using ChatyChaty.Model.Repositories.MessageRepository;
using ChatyChaty.Model.Repositories.UserRepository;
using ChatyChaty.Services.NotificationServices;
using ChatyChaty.Services.PictureServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.MessageServices
{
    /// <summary>
    /// Class that handle the messaging logic
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly IChatRepository chatRepository;
        private readonly INotificationHandler notificationHandler;
        private readonly IPictureProvider pictureProvider;

        public MessageService(IMessageRepository messageRepository,
            IUserRepository userRepository,
            IChatRepository chatRepository,
            INotificationHandler notificationHandler,
            IPictureProvider pictureProvider)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.notificationHandler = notificationHandler;
            this.pictureProvider = pictureProvider;
            this.chatRepository = chatRepository;
        }

        /// <summary>
        /// Check if a message was delivered
        /// </summary>
        /// <param name="userId">The Id of the user who own the message</param>
        /// <param name="messageId">The Id of the message to be checked</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the UserId doesn't exist</exception>
        public async Task<IsDeliveredModel> IsDelivered(long userId, long messageId)
        {
            var message = await messageRepository.GetMessageAsync(messageId);
            if (message == null)
            {
                return new IsDeliveredModel
                {
                    Error = "No such a message Id"
                };
            }
            if (message.SenderId != userId)
            {
                return new IsDeliveredModel
                {
                    Error = "User is not the sender of this message"
                };
            }
            return new IsDeliveredModel
            {
                IsDelivered = message.Delivered
            };
        }
        /*
        public async Task<IEnumerable<DeliveredMessageIdForChat>> GetDeliveredMessageIdForChats(long userId)
        {
            messageRepository
        }
        */

        /// <summary>
        /// Send message with the provided conversation Id 
        /// </summary>
        /// <param name="ConversationId">The conversation Id when can we get from the get profile</param>
        /// <param name="SenderId">The sender Id</param>
        /// <param name="MessageBody">The message</param>
        /// <returns>Return the sent message back</returns>
        public async Task<SendMessageModel> SendMessage(long ConversationId, long SenderId, string MessageBody)
        {
            //check if the conversation exist
            var conversation = await chatRepository.GetConversationAsync(ConversationId);
            if (conversation == null)
            {
                return new SendMessageModel { Error = "Invalid ChatId" };
            }

            //check if the user is part of a conversation and find the receiver
            long ReceiverId;
            if (conversation.FirstUserId == SenderId)
            {
                ReceiverId = conversation.SecondUserId;
            }
            else if (conversation.SecondUserId == SenderId)
            {
                ReceiverId = conversation.FirstUserId;
            }
            else
            {
                return new SendMessageModel { Error = "Invalid ChatId" };
            }

            var message = new Message(MessageBody, conversation.Id, SenderId);

            var returnedMessage = await messageRepository.AddMessageAsync(message);

            await notificationHandler.UserGotNewMessageAsync((ReceiverId, returnedMessage.Id));

            return new SendMessageModel { Message = returnedMessage };
        }

        /// <summary>
        /// create or get a conversation between 2 users
        /// </summary>
        /// <param name="senderId">First user Id</param>
        /// <param name="receiverId">Second user Id</param>
        /// <returns>A long that represent the created conversation Id</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when one or both users don't exist</exception>
        [Obsolete("use new conversation in account manager instead")]
        public async Task<long> NewConversation(long senderId, long receiverId)
        {
            var sender = await userRepository.GetUserAsync(senderId);
            var reciver = await userRepository.GetUserAsync(receiverId);
            if (sender == null || reciver == null)
            {
                throw new ArgumentOutOfRangeException("Invalid IDs");
            }

            var conversation = await chatRepository.GetConversationForUsersAsync(sender.Id, reciver.Id);

            await notificationHandler.UsersGotChatUpdateAsync((sender.Id,reciver.Id));

            return conversation.Id;
        }

        public async Task<GetNewMessagesModel> GetNewMessages(long userId, long lastMessageId)
        {
            var userConversationsId = await chatRepository.GetUserConversationIdsAsync(userId);
            var newMessages = await messageRepository.GetMessagesFromConversationIdsAsync(lastMessageId, userConversationsId);
            //Mark messages as read
            var markMessages = new List<Message>();
            foreach (var message in newMessages)
            {
                if (message.SenderId != userId && !message.Delivered)
                {
                    markMessages.Add(message);
                }
            }
            await messageRepository.MarkAsReadAsync(markMessages);
            await notificationHandler.UsersGotMessageDeliveredAsync(markMessages.Select(m => (m.SenderId, m.Id)).ToArray());
            //error in get new message is redundant currently
            return new GetNewMessagesModel { Messages = newMessages };
        }


        /// <summary>
        /// Get a list of conversations for a user
        /// </summary>
        /// <remarks>throws exception if the user doesn't exist</remarks>
        /// <param name="userId">The userId who have the conversations</param>
        /// <returns>a list of conversations</returns>
        public async Task<IEnumerable<ConversationInfo>> GetConversations(long userId)
        {
            var user = await userRepository.GetUserAsync(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Ids");
            };

            var conversations = await chatRepository.GetUserConversationsWithUsersAsync(userId);

            var response = new List<ConversationInfo>();

            foreach (var conversation in conversations)
            {
                AppUser SecondUser;
                if (user.Id == conversation.FirstUserId)
                {
                    SecondUser = conversation.SecondUser;
                }
                else if (user.Id == conversation.SecondUserId)
                {
                    SecondUser = conversation.FirstUser;
                }
                else throw new Exception("Invalid Conversation");

                response.Add(new ConversationInfo
                {
                    ConversationId = conversation.Id,
                    SecondUserDisplayName = SecondUser.DisplayName,
                    SecondUserUsername = SecondUser.UserName,
                    SecondUserId = SecondUser.Id,
                    SecondUserPhoto = await pictureProvider.GetPhotoURL(SecondUser.Id, SecondUser.UserName)
                });
            }
            return response;
        }
    }
}
