using ChatyChatyClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Services
{
    public interface IMessageService
    {
        public IList<Message> GetMessagesForChat(long chatId);
        public IList<Chat> GetChatList();
        public Message SendMessage();
        public void FindUser(string username);
    }
}
