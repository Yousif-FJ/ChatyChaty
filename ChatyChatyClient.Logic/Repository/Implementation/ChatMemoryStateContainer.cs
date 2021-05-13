using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Repository.Implementation
{
    public class ChatMemoryStateContainer : IChatStateContainer
    {
        private IList<Chat> chats;
        public IList<Chat> GetChats()
        {
            return chats;
        }

        public void SetChats(IList<Chat> chats)
        {
            this.chats = chats;
        }

        public void UpdateChat(Chat chat)
        {
            var toUpdateChat = chats.FirstOrDefault(c => c.Id == chat.Id);
            if (toUpdateChat is null)
            {
                chats.Add(chat);
            }
            else
            {
                chats.Remove(toUpdateChat);
                chats.Add(chat);
            }
        }
    }
}
