using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessageRepository;
using ChatyChaty.Model.Messaging_model;
using ChatyChaty.Model.NotficationHandler;
using Microsoft.EntityFrameworkCore;
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

        public MessageService(IMessageRepository messageRepository, INotificationHandler notificationHandler)
        {
            this.messageRepository = messageRepository;
            this.notificationHandler = notificationHandler;
        }

        /// <summary>
        /// Check if a message was delivered
        /// </summary>
        /// <param name="UserId">The Id of the user who own the message</param>
        /// <param name="MessageId">The Id of the message to be checked</param>
        /// <remarks>Return null if the the message doesn't exist or The User doesn't own the message</remarks>
        /// <returns>A bool if the message is delivered or not</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the UserId doesn't exist</exception>
        public async Task<bool?> IsDelivered(long UserId, long MessageId)
        {
            var User = await messageRepository.GetUser(UserId);
            if (User == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Id");
            }

            var Message = await messageRepository.GetMessage(MessageId);
            if (Message == null || Message.SenderId != User.Id)
            {
                return null;
            }
            return Message.Delivered;
        }

        /// <summary>
        /// Send message with the provided conversation Id 
        /// </summary>
        /// <remarks>the method return null if the conversation doesn't exist or if the user doesn't own the conversation</remarks>
        /// <param name="ConversationId">The conversation Id when can we get from the get profile</param>
        /// <param name="SenderId">The sender Id</param>
        /// <param name="MessageBody">The message</param>
        /// <returns>Return the sent message back</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the UserId doesn't exist</exception>
        public async Task<Message> SendMessage(long ConversationId, long SenderId, string MessageBody)
        {
            var conversation = await messageRepository.GetConversation(ConversationId);
            if (conversation == null)
            {
                return null;
            }
            var Sender = await messageRepository.GetUser(SenderId);
            if (Sender == null)
            {
                throw new ArgumentOutOfRangeException("Invalid IDs");
            }

            if (!await messageRepository.IsConversationForUser(conversation.Id, SenderId))
            {
                return null;
            }

            if (conversation.FirstUserId == Sender.Id)
            {
                await notificationHandler.UserGotNewMessage(conversation.SecondUserId);
            }
            else
            {
                await notificationHandler.UserGotNewMessage(conversation.FirstUserId);
            }

            var Message = new Message
            {
                Body = MessageBody,
                SenderId = Sender.Id,
                Delivered = false,
                ConversationId = conversation.Id
            };

            var ReturnedMessage = await messageRepository.AddMessage(Message);

            return ReturnedMessage;
        }

        /// <summary>
        /// create or get a conversation between 2 users
        /// </summary>
        /// <param name="SenderId">First user Id</param>
        /// <param name="ReceiverId">Second user Id</param>
        /// <returns>A long that represent the created conversation Id</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when one or both users don't exist</exception>
        public async Task<long> NewConversation(long SenderId, long ReceiverId)
        {
            var SenderDB = await messageRepository.GetUser(SenderId);
            var ReciverDB = await messageRepository.GetUser(ReceiverId);
            if (SenderDB == null || ReciverDB == null)
            {
                throw new ArgumentOutOfRangeException("Invalid IDs");
            }

            var conversation = await messageRepository.FindConversationForUsers(SenderDB.Id, ReciverDB.Id);

            if (conversation == null)
            {
                return (await messageRepository.CreateConversationForUsers(SenderDB.Id, ReciverDB.Id)).Id;
            }

            await notificationHandler.UserGotChatUpdate(ReciverDB.Id);
            
            return conversation.Id;
        }

        public async Task<IEnumerable<Message>> GetNewMessages(long UserId, long LastMessageId)
        {
            var UserConversationsId = messageRepository.GetUserConversationIds(UserId);
            var NewMessages = await messageRepository.GetMessagesFromConversationIds(LastMessageId, UserConversationsId);

            await messageRepository.MarkAsRead(NewMessages);
            return NewMessages;
        }


        /// <summary>
        /// Get a list of conversations for a user
        /// </summary>
        /// <remarks>throws exception if the user doesn't exist</remarks>
        /// <param name="UserId">The userId who have the conversations</param>
        /// <returns>a list of conversations</returns>
        public async Task<IEnumerable<ConversationInfo>> GetConversations(long UserId)
        {
            var user = await messageRepository.GetUser(UserId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Invalid IDs");
            };

            var conversations = await messageRepository.GetUserConversationsWithUsers(UserId);

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
                    SecondUserId = SecondUser.Id
                });

            }
            return response;
    }
            

    /// <summary>
    /// Check if there is new message for a user
    /// </summary>
    /// <remarks>Return null if the user doesn't exist or doesn't own the message</remarks>
    /// <param name="UserId">The userId who is requesting the update</param>
    /// <param name="LastMessageId">The last messaegId</param>
    /// <returns>A bool whether there is new message</returns>
    public async Task<bool?> CheckForNewMessages(long UserId, long LastMessageId)
        {

            var UserConversationsId = messageRepository.GetUserConversationIds(UserId);

            return await messageRepository.IsThereNewMessageInConversationIds(LastMessageId,UserConversationsId) ;
        }
    }
}
