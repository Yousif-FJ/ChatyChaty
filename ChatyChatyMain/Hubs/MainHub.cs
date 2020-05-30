using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs
{
    public class MainHub : Hub
    {
        public Task SendMessageToCaller(string user, string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", user , message);
        }
    }
}
