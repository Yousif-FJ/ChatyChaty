using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.RepositoryImplementation
{
    public class ChatStateContainer : IChatStateContainer
    {
        private List<Chat> chats;

        public void AddChat(Chat chat)
        {
            if (chat is null)
            {
                throw new ArgumentNullException(nameof(chat));
            }

            chats.Add(chat);
        }

        public Chat GetChat(string chatId)
        {
            if (chats is null)
            {
                return null;
            }
            return chats.FirstOrDefault(chat => chat.Id == chatId);
        }

        public IList<Chat> GetChats()
        {
            return chats;
        }

        public void SetChats(IList<Chat> chats)
        {
            if (chats is null)
            {
                throw new ArgumentNullException(nameof(chats));
            }

            this.chats = new List<Chat>(chats);
        }
    }
}
