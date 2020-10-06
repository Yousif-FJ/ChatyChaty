using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Services.MessageServices;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    /// <summary>
    /// This class is used when the hub is called outside of the MainHub classes
    /// </summary>
    public class HubHelper
    {
        private readonly IHubContext<MainHub, IChatClient> hubContext;
        private readonly IMessageService messageService;
        private readonly HubClientsStateManager hubClients;

        public HubHelper(IHubContext<MainHub, IChatClient> hubContext,
            IMessageService messageService,
            HubClientsStateManager hubClients
            )
        {
            this.hubContext = hubContext;
            this.messageService = messageService;
            this.hubClients = hubClients;
        }
        /// <summary>
        /// Method called to send message updates to a connected client, return false if the client is not connected
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> SendUpdateAsync(long userId)
        {
            //check if client is connected, i.e. registered in client state
            var clientState = hubClients.GetClient(userId);
            if (clientState == null)
            {
                return false;
            }
            var lastMessageId = clientState.LastMessageId;

            //get new messages form message service
            var result = await messageService.GetNewMessages(userId, lastMessageId);

            //send update to client about new messages (using this extension method)
            hubContext.Clients.SendMessageUpdatesAsync(result, userId, ref lastMessageId);

            //update client list
            hubClients.AddUpdateClient(userId, lastMessageId);
            return true;
        }
    }
}
