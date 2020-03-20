using ChatyChaty.Model.DBModel;
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

        public Task<IEnumerable<Message>> GetNewMessages(long UserId, long LastMessageID)
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> IsDelivered(long UserId, long MessageId)
        {
            var User = await dBContext.Users.FindAsync(UserId);
            if (User == null)
            {
                throw new ArgumentOutOfRangeException("Invalid IDs");
            }
            var Message = await dBContext.Messages.FindAsync(MessageId);
            if (Message == null || Message.SenderId != User.Id)
            {
                return null;
            }
            return Message.Delivered;
        }

        public async Task<Message> SendMessage(long ConversationId, long SenderId, string MessageBody)
        {
            var conversation = await dBContext.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == ConversationId);
            if (conversation == null )
            {
                return null;
            }
            var Sender = await dBContext.Users.FindAsync(SenderId);
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

        public async Task<Message> SendNewMessage(long SenderId, long ReceiverId, string MessageBody)
         {
            if (MessageBody == null)
            {
                throw new ArgumentNullException();
            }
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
                    Messages = new List<Message>()
                };
            }

            conversation.Messages.Add(new Message
            {
                Body = MessageBody,
                SenderId = SenderDB.Id,
                Delivered = false
            });

            var resultConv = await dBContext.Conversations.AddAsync(conversation);
            await dBContext.SaveChangesAsync();

            return resultConv.Entity.Messages.Last();
        }


    }
}
