using ChatyChatyClient.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.ViewModel
{
    public class ChatViewModel
    {
        public ChatViewModel(UserProfile selfProfile, Chat chatInfo)
        {
            SelfProfile = selfProfile ?? throw new ArgumentNullException(nameof(selfProfile));
            ChatInfo = chatInfo ?? throw new ArgumentNullException(nameof(chatInfo));
        }

        public UserProfile SelfProfile { get; }
        public Chat ChatInfo { get; }
    }
}
