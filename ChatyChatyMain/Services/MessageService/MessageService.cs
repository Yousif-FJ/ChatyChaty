using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessageRepository;
using ChatyChaty.Model.MessagingModel;
using ChatyChaty.Model.NotficationHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository messageRepository;
        private readonly INotificationHandler notificationHandler;
        private readonly IPictureProvider pictureProvider;


        public MessageService(IMessageRepository messageRepository, 
            INotificationHandler notificationHandler, 
            IPictureProvider pictureProvider)
        {
            this.messageRepository = messageRepository;
            this.notificationHandler = notificationHandler;
            this.pictureProvider = pictureProvider;

        }

        /// <summary>
        /// Check if a message was delivered
        /// </summary>
        /// <param name="userId">The Id of the user who own the message</param>
        /// <param name="messageId">The Id of the message to be checked</param>
        /// <remarks>Return null if the the message doesn't exist or The User doesn't own the message</remarks>
        /// <returns>A bool if the message is delivered or not</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the UserId doesn't exist</exception>
        public async Task<IsDeliveredModel> IsDelivered(long userId, long messageId)
        {
            var user = await messageRepository.GetUserAsync(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Id");
            }

            var message = await messageRepository.GetMessageAsync(messageId);
            if (message == null )
            {
                return new IsDeliveredModel
                {
                    Error = "No such a message Id"
                };
            }
            if (message.SenderId != user.Id)
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

        /// <summary>
        /// Send message with the provided conversation Id 
        /// </summary>
        /// <param name="ConversationId">The conversation Id when can we get from the get profile</param>
        /// <param name="SenderId">The sender Id</param>
        /// <param name="MessageBody">The message</param>
        /// <returns>Return the sent message back</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the UserId doesn't exist</exception>
        public async Task<SendMessageModel> SendMessage(long ConversationId, long SenderId, string MessageBody)
        {
            var conversation = await messageRepository.GetConversationAsync(ConversationId);
            if (conversation == null)
            {
                return new SendMessageModel { Error = "Invalid ChatId" };
            }
            var Sender = await messageRepository.GetUserAsync(SenderId);
            if (Sender == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Ids");
            }

            if (!await messageRepository.IsConversationForUserAsync(conversation.Id, SenderId))
            {
                return new SendMessageModel { Error = "Invalid ChatId" };
            }

            if (conversation.FirstUserId == Sender.Id)
            {
                await notificationHandler.UserGotNewMessage(conversation.SecondUserId);
            }
            else
            {
                await notificationHandler.UserGotNewMessage(conversation.FirstUserId);
            }

            var message = new Message
            {
                Body = MessageBody,
                SenderId = Sender.Id,
                Delivered = false,
                ConversationId = conversation.Id
            };

            var returnedMessage = await messageRepository.AddMessageAsync(message);

            return new SendMessageModel { Message = returnedMessage };
        }

        /// <summary>
        /// create or get a conversation between 2 users
        /// </summary>
        /// <param name="senderId">First user Id</param>
        /// <param name="receiverId">Second user Id</param>
        /// <returns>A long that represent the created conversation Id</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when one or both users don't exist</exception>
        public async Task<long> NewConversation(long senderId, long receiverId)
        {
            var senderDB = await messageRepository.GetUserAsync(senderId);
            var reciverDB = await messageRepository.GetUserAsync(receiverId);
            if (senderDB == null || reciverDB == null)
            {
                throw new ArgumentOutOfRangeException("Invalid IDs");
            }

            var conversation = await messageRepository.FindConversationForUsersAsync(senderDB.Id, reciverDB.Id);
            if (conversation == null)
            {
                conversation = await messageRepository.CreateConversationForUsersAsync(senderDB.Id, reciverDB.Id);
            }

            await notificationHandler.UserGotChatUpdate(reciverDB.Id);
            
            return conversation.Id;
        }

        public async Task<GetNewMessagesModel> GetNewMessages(long userId, long lastMessageId)
        {
            var userConversationsId = messageRepository.GetUserConversationIds(userId);
            var newMessages = await messageRepository.GetMessagesFromConversationIdsAsync(lastMessageId, userConversationsId);
            //Mark messages as read
            var markMessages = new List<Message>();
            foreach (var message in newMessages)
            {
                if (message.SenderId != userId && !message.Delivered )
                {
                    markMessages.Add(message);
                }
            }
            await messageRepository.MarkAsReadAsync(markMessages);
            await notificationHandler.NotifySenderMessagesWhereDelivered(markMessages.Select(m => m.SenderId));
            if (newMessages != null)
            {
                return new GetNewMessagesModel { Messages = newMessages };
            }
            else
            {
                return new GetNewMessagesModel { Error = "Invalid messageId or no new messages" };
            }
        }


        /// <summary>
        /// Get a list of conversations for a user
        /// </summary>
        /// <remarks>throws exception if the user doesn't exist</remarks>
        /// <param name="userId">The userId who have the conversations</param>
        /// <returns>a list of conversations</returns>
        public async Task<IEnumerable<ConversationInfo>> GetConversations(long userId)
        {
            var user = await messageRepository.GetUserAsync(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Ids");
            };

            var conversations = await messageRepository.GetUserConversationsWithUsersAsync(userId);

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
            

    /// <summary>
    /// Check if there is new message for a user
    /// </summary>
    /// <remarks>Return null if the user doesn't exist or doesn't own the message</remarks>
    /// <param name="userId">The userId who is requesting the update</param>
    /// <param name="lastMessageId">The last messaegId</param>
    /// <returns>A bool whether there is new message</returns>
    public async Task<bool?> CheckForNewMessages(long userId, long lastMessageId)
        {
            var userConversationsId = messageRepository.GetUserConversationIds(userId);
            return await messageRepository.IsThereNewMessageInConversationIdsAsync(lastMessageId,userConversationsId) ;
        }
    }
}
