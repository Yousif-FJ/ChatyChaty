using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Domain.Services.MessageServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    /// <summary>
    /// Main sigalR hub that handles WebSocket method calles
    /// </summary>
    [Authorize]
    public class MainHub : Hub<IChatClient>
    {
        public MainHub(IHubSessions hubClients)
        {
            this.hubClients = hubClients;
        }
        
        private readonly IHubSessions hubClients;

        public override async Task OnConnectedAsync()
        {
            var userId = long.Parse(Context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            //update client list
            hubClients.AddClient(userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            hubClients.RemoveClient(long.Parse(userId));
            await base.OnDisconnectedAsync(exception);
        }
    }
}
