using ChatyChatyClient.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Repository.Interfaces
{
    public interface IChatStateContainer
    {
        IList<Chat> GetChats();
        void SetChats(IList<Chat> chats);
        void UpdateChat(Chat chat);
    }
}
