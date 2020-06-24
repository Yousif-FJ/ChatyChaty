using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs
{

    [Authorize]
    public class MainHub : Hub
    {
        public Task SendMessageToCaller(string user, string message)
        {
            var userId = Context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            return Clients.Caller.SendAsync("ReceiveMessage", user , message);
        }
    }
}
