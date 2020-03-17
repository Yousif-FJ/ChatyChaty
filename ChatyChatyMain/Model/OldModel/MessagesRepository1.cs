using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.OldModel
{
    public class MessagesRepository1 :IMessageRepository1
    {
        private readonly ChatyChatyContext chatyChatyContext;

        public MessagesRepository1(ChatyChatyContext chatyChatyContext) 
        {
            this.chatyChatyContext = chatyChatyContext;
        }

        public IEnumerable<Message1> DeleteAllMessages()
        {
            var messages = chatyChatyContext.MessagesSet;
            chatyChatyContext.MessagesSet.RemoveRange(messages);
            chatyChatyContext.SaveChanges();
            return messages;
        }

        public IEnumerable<Message1> GetAllMessages()
        {
            return chatyChatyContext.MessagesSet;
        }

        public IEnumerable<Message1> GetNewMessages(long ID)
        {
            var LastID = chatyChatyContext.MessagesSet.Max(m => m.ID);
            if (LastID == ID)
            {
                return new List<Message1>();
            }
            else if (LastID >=ID)
            {
                return chatyChatyContext.MessagesSet.Where(message => message.ID > ID);
            }
            else
            {
                return null;
            }
        }

        public Message1 NewMessage(Message1 message)
        {
            chatyChatyContext.MessagesSet.Add(message);
            chatyChatyContext.SaveChanges();
            return message;
        }
    }
}
