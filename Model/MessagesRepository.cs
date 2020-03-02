using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class MessagesRepository :IMessageRepository
    {
        private readonly ChatyChatyContext chatyChatyContext;

        public MessagesRepository(ChatyChatyContext chatyChatyContext) 
        {
            this.chatyChatyContext = chatyChatyContext;
        }

        public IEnumerable<Message> GetAllMessages()
        {
            return chatyChatyContext.MessagesSet;
        }

        public Message NewMessage(Message message)
        {
            chatyChatyContext.MessagesSet.Add(message);
            return message;
        }
    }
}
