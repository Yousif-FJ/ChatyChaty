using ChatyChatyClient.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.ViewModel
{
    public class ChatListViewModel
    {
        public ChatListViewModel(IList<Chat> chats)
        {
            Chats = chats ?? throw new ArgumentNullException(nameof(chats));
        }
        public IList<Chat> Chats { get; }
    }
}
