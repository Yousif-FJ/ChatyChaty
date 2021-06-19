using ChatyChaty.Domain.ApplicationExceptions;
using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.NotficationRequests;
using ChatyChaty.Domain.Services.ScopeServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ChatyChaty.Domain.Services.MessageServices
{
    /// <summary>
    /// Class that handle the messaging logic
    /// </summary>
    public class MessageService : IMessageService
    {
        private const int MaxMessagesNumber = 1000;
        private readonly IMessageRepository messageRepository;
        private readonly IChatRepository chatRepository;
        private readonly IFireAndForgetService fireAndForget;

        public MessageService(
            IMessageRepository messageRepository,
            IChatRepository chatRepository,
            IFireAndForgetService fireAndForget
            )
        {
            this.messageRepository = messageRepository;
            this.chatRepository = chatRepository;
            this.fireAndForget = fireAndForget;
        }

        /// <summary>
        /// Check if a message was delivered
        /// </summary>
        /// <exception cref="InvalidEntityIdException"></exception>
        public async Task<bool> IsDelivered(UserId userId, MessageId messageId)
        {
            var message = await messageRepository.GetAsync(messageId);

            if (message is null || message.SenderId != userId)
            {
                throw new InvalidEntityIdException(messageId);
            }

            return message.Delivered;
        }

        /// <summary>
        /// Send message with the provided conversation Id 
        /// </summary>
        /// <exception cref="InvalidEntityIdException"></exception>
        public async Task<Message> SendMessage(ConversationId conversationId, UserId senderId, string MessageBody)
        {
            if (senderId is null)
            {
                throw new ArgumentNullException(nameof(senderId));
            }

            if (string.IsNullOrEmpty(MessageBody))
            {
                throw new ArgumentException($"'{nameof(MessageBody)}' cannot be null or empty.", nameof(MessageBody));
            }

            //check if the conversation exist
            var conversation = await chatRepository.GetAsync(conversationId);
            if (conversation is null)
            {
                throw new InvalidEntityIdException(conversationId);
            }

            UserId receiverId = conversation.FindReceiverId(senderId);
            if (receiverId is null)
            {
                throw new InvalidEntityIdException(conversationId);
            }

            var message = new Message(MessageBody, conversation.Id, senderId);

            await messageRepository.AddAsync(message);

            fireAndForget.RunActionWithoutWaitingAsync<IMediator>(mediator => mediator.Send(new UserGotNewMessageAsync((receiverId, message.Id))));
            
            return message;
        }

        /// <summary>
        /// get messages that are newer that the provided lastMessageId
        /// </summary>
        public async Task<IList<Message>> GetNewMessages(UserId userId, DateTime? lastMessageTime)
        {
            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            IList<Message> messages;
            if (lastMessageTime is null)
            {
                messages = await messageRepository.GetAllAsync(userId);
            }
            else
            {
                messages = await messageRepository.GetNewAsync(lastMessageTime.Value, userId);
            }

            var markedMessages = FindAndMarkDeliveredMessages(messages, userId);

            if (messages.Count > MaxMessagesNumber)
            {
                fireAndForget.RunActionWithoutWaitingAsync<IMessageRepository>(messageRepository => messageRepository.RemoveOverLimit(userId));
            }

            fireAndForget.RunActionWithoutWaitingAsync<IMessageRepository>(messageRepository => messageRepository.UpdateRangeAsync(markedMessages));

            fireAndForget.RunActionWithoutWaitingAsync<IMediator>(mediator =>
                mediator.Send(new UsersGotMessageStatusUpdateAsync(markedMessages.Select(m => (m.SenderId, m.ConversationId, m.Id)).ToArray())));

            return messages;
        }

        /// <summary>
        /// get messages of a specific chat
        /// <exception cref="InvalidEntityIdException"></exception>
        public async Task<IList<Message>> GetMessageForChat(UserId userId, ConversationId conversationId)
        {
            var chat = await chatRepository.GetAsync(conversationId);

            if (chat is null)
            {
                throw new InvalidEntityIdException(conversationId);
            }

            if (chat.FirstUserId != userId || chat.SecondUserId != userId)
            {
                throw new InvalidEntityIdException(conversationId);
            }

            var messages = await messageRepository.GetForChatAsync(conversationId);

            var markedMessages = FindAndMarkDeliveredMessages(messages, userId);

            if (messages.Count > MaxMessagesNumber)
            {
                fireAndForget.RunActionWithoutWaitingAsync<IMessageRepository>(messageRepository => messageRepository.RemoveOverLimit(userId));
            }

            fireAndForget.RunActionWithoutWaitingAsync<IMessageRepository>(messageRepository => messageRepository.UpdateRangeAsync(markedMessages));

            fireAndForget.RunActionWithoutWaitingAsync<IMediator>(mediator =>
                mediator.Send(new UsersGotMessageStatusUpdateAsync(markedMessages.Select(m => (m.SenderId, m.ConversationId, m.Id)).ToArray())));

            return messages;
        }

        private static IList<Message> FindAndMarkDeliveredMessages(IEnumerable<Message> messages, UserId userId)
        {
            var result = new List<Message>();
            foreach (var message in messages)
            {
                if (message.SenderId != userId && !message.Delivered)
                {
                    message.MarkAsDelivered();
                    result.Add(message);
                }
            }
            return result;
        }
    }
}
