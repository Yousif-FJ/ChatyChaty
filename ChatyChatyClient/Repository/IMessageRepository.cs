using ChatyChatyClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Repository
{
    public interface IMessageRepository
    {
        public IList<Chat> GetChatList();
        public IList<Message> GetMessagesForChat(long chatId);
        public void AddMessage(string messageBody, string sender, long chatId);
    }
}
