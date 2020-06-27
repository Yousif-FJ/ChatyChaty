using ChatyChaty.ControllerSchema.v3;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v1
{
    [Authorize]
    public class MainHub : Hub<IChatClient>
    {
        private readonly IMessageService messageService;

        public MainHub(IMessageService messageService)
        {
            this.messageService = messageService;
        }
        
        public Task SendTest(string message)
        {
            var userId = Context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            return Clients.Caller.TestResponse(message);
        }
    }
}
