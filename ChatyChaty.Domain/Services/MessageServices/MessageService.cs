﻿using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Model.MessagingModel;
using ChatyChaty.Domain.Services.NotficationServices.Handler;
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

        public MessageService(IMessageRepository messageRepository,
            IChatRepository chatRepository,
            IMediator mediator
            )
        {
            this.messageRepository = messageRepository;
            this.mediator = mediator;
            this.chatRepository = chatRepository;
        }

        /// <summary>
        /// Check if a message was delivered
        /// </summary>
        /// <param name="userId">The Id of the user who own the message</param>
        /// <param name="messageId">The Id of the message to be checked</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the UserId doesn't exist</exception>
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

            await mediator.Send(new UserGotNewMessageAsync((ReceiverId, returnedMessage.Id)));

            return new SendMessageModel { Message = returnedMessage };
        }


        public async Task<GetNewMessagesModel> GetNewMessages(long userId, long lastMessageId)
        {
            var newMessages = await messageRepository.GetMessagesForUser(lastMessageId, userId);
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
            await messageRepository.UpdateMessagesAsync(markMessages);
            await mediator.Send(new UsersGotMessageStatusUpdateAsync(markMessages.Select(m => (m.SenderId,m.ConversationId, m.Id)).ToArray()));
 //error in get new message is redundant currently
            return new GetNewMessagesModel { Messages = newMessages };
        }
    }
}