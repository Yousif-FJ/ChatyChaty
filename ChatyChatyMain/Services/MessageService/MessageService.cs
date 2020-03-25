﻿using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.Messaging_model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public class MessageService : IMessageService
    {
        private readonly ChatyChatyContext dBContext;

        public MessageService(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
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
            var User = await dBContext.Users.FindAsync(UserId);
            if (User == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Id");
            }
            var Message = await dBContext.Messages.FindAsync(MessageId);
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
            var conversation = await dBContext.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == ConversationId);
            if (conversation == null)
            {
                return null;
            }
            var Sender = await dBContext.Users.FindAsync(SenderId);
            if (Sender == null)
            {
                throw new ArgumentOutOfRangeException("Invalid IDs");
            }
            if (conversation.FirstUserId != Sender.Id && conversation.SecondUserId != Sender.Id)
            {
                return null;
            }

            conversation.Messages.Add(new Message
            {
                Body = MessageBody,
                SenderId = Sender.Id,
                Delivered = false
            });

            var resultConv = dBContext.Conversations.Update(conversation);
            await dBContext.SaveChangesAsync();

            return resultConv.Entity.Messages.Last();
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
            var SenderDB = await dBContext.Users.FindAsync(SenderId);
            var ReciverDB = await dBContext.Users.FindAsync(ReceiverId);
            if (SenderDB == null || ReciverDB == null)
            {
                throw new ArgumentOutOfRangeException("Invalid IDs");
            }

            var conversation = await dBContext.Conversations.FirstOrDefaultAsync(
                c => c.FirstUserId == SenderDB.Id || c.SecondUserId == SenderDB.Id
            );

            if (conversation == null)
            {
                conversation = await dBContext.Conversations.FirstOrDefaultAsync(
                    c => c.FirstUserId == ReciverDB.Id || c.SecondUserId == ReciverDB.Id
                );
            }
            if (conversation == null)
            {
                conversation = new Conversation()
                {
                    FirstUserId = SenderDB.Id,
                    SecondUserId = ReciverDB.Id,
                };
            var resultConv = await dBContext.Conversations.AddAsync(conversation);
            await dBContext.SaveChangesAsync();
            return resultConv.Entity.Id;
            }
            return conversation.Id;
        }

        public async Task<IEnumerable<Message>> GetNewMessages(long UserId, long LastMessageId)
        {
            var UserConversationsId = dBContext.Conversations
                .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
                .Select(c => c.Id);
            var NewMessages = dBContext.Messages.Where(
                m => m.Id > LastMessageId &&
                UserConversationsId.Any(id => id == m.ConversationId)
                ).Include(c => c.Sender);

            foreach (var Message in NewMessages)
            {
                Message.Delivered = true;
            }
            dBContext.Messages.UpdateRange(NewMessages);
            await dBContext.SaveChangesAsync();
            return NewMessages;
        }


        /// <summary>
        /// Get the conversation object of a user
        /// </summary>
        /// <remarks>Return null if the user doesn't exist or doesn't own the conversation</remarks>
        /// <param name="UserId">The userId who have the conversation</param>
        /// <param name="conversationId">The requested conversation</param>
        /// <returns>conversation of the given user</returns>
        public async Task<ConversationInfo> GetConversationInfo(long UserId, long conversationId)
        {
            var conversation = await dBContext.Conversations.FindAsync(conversationId);
            if (conversation == null)
            {
                return null;
            }
            AppUser SecondUser;
            if (conversation.FirstUserId == UserId)
            {
                SecondUser = await dBContext.Users.FindAsync(conversation.SecondUserId); 
            }
            else if (conversation.SecondUserId == UserId)
            {
                SecondUser = await dBContext.Users.FindAsync(conversation.FirstUserId);
            }
            else
            {
                return null;
            }

            var response = new ConversationInfo
            {
                ConversationId = conversation.Id,
                SecondUserDisplayName = SecondUser.DisplayName,
                SecondUserUsername = SecondUser.UserName,
                SecondUserId = SecondUser.Id
            };

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

            var UserConversationsId = dBContext.Conversations
              .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
              .Select(c => c.Id);

            var IsThereNewMessage = await dBContext.Messages.AnyAsync(
                 m => m.Id > LastMessageId &&
                UserConversationsId.Any(id => id == m.ConversationId)
                 );

            return IsThereNewMessage;
        }
    }
}
