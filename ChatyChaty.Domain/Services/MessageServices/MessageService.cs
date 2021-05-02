using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Model.MessagingModel;
using ChatyChaty.Domain.Services.NotficationRequests;
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
        private readonly IMessageRepository messageRepository;
        private readonly IChatRepository chatRepository;
        private readonly IMediator mediator;

        public MessageService(
            IMessageRepository messageRepository,
            IChatRepository chatRepository,
            IMediator mediator)
        {
            this.messageRepository = messageRepository;
            this.mediator = mediator;
            this.chatRepository = chatRepository;
        }

        /// <summary>
        /// Check if a message was delivered
        /// </summary>
        public async Task<IsDeliveredModel> IsDelivered(UserId userId, MessageId messageId)
        {
            var message = await messageRepository.GetAsync(messageId);
            if (message is null)
            {
                return new IsDeliveredModel
                {
                    Error = "Message not found"
                };
            }
            if (!message.SenderId.Equals(userId))
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
        public async Task<SendMessageModel> SendMessage(ConversationId conversationId, UserId senderId, string MessageBody)
        {
            //check if the conversation exist
            var conversation = await chatRepository.GetAsync(conversationId);
            if (conversation is null)
            {
                return new SendMessageModel { Error = "Invalid ChatId" };
            }

            UserId receiverId = conversation.FindReceiverId(senderId);
            if (receiverId is null)
            {
                return new SendMessageModel { Error = "Invalid ChatId" };
            }

            var message = new Message(MessageBody, conversation.Id, senderId);
            await messageRepository.AddAsync(message);

            await mediator.Send(new UserGotNewMessageAsync((receiverId, message.Id)));

            return new SendMessageModel { Message = message };
        }


        public async Task<GetMessagesModel> GetNewMessages(UserId userId, MessageId lastMessageId)
        {
            if (userId is null)
            {
                return new GetMessagesModel { Error = "UserId is null" };
            }

            IEnumerable<Message> newMessages;
            if (lastMessageId is null)
            {
                newMessages = await messageRepository.GetAllAsync(userId);
            }
            else
            {
                newMessages = await messageRepository.GetNewAsync(lastMessageId, userId);
            }

            //Mark messages as read
            var markMessages = new List<Message>();
            foreach (var message in newMessages)
            {
                if (message.SenderId != userId && !message.Delivered)
                {
                    message.MarkAsDelivered();
                    markMessages.Add(message);
                }
            }
            await messageRepository.UpdateRangeAsync(markMessages);

            await mediator.Send(new UsersGotMessageStatusUpdateAsync(markMessages.Select(m => (m.SenderId,m.ConversationId, m.Id)).ToArray()));

            return new GetMessagesModel { Messages = newMessages };
        }

        public async Task<GetMessagesModel> GetMessageForChat(UserId userId, ConversationId conversationId)
        {
            var chat = await chatRepository.GetAsync(conversationId);

            if (chat is null)
            {
                return new GetMessagesModel { Error = "Invalid chat Id" };
            }

            if (chat.FirstUserId != userId || chat.SecondUserId != userId)
            {
                return new GetMessagesModel { Error = "Invalid chat Id" };
            }

            var messages = await messageRepository.GetForChatAsync(conversationId);

            return new GetMessagesModel { Messages = messages };
        }
    }
}
