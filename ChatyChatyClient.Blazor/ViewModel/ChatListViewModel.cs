using ChatyChatyClient.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.ViewModel
{
    public class ChatListViewModel
    {
        public ChatListViewModel()
        {
            Chats = new List<Chat>();
        }
        public IList<Chat> Chats { get; set; }
    }
}
