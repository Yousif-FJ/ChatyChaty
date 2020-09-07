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
        private readonly HubClientsStateManager stateManager;

        public HubHelper(IHubContext<MainHub, IChatClient> hubContext,
            IMessageService messageService,
            HubClientsStateManager stateManager
            )
        {
            this.hubContext = hubContext;
            this.messageService = messageService;
            this.stateManager = stateManager;
        }
        /// <summary>
        /// Method called to send message updates to a connected client, return false if the client is not connected
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> SendUpdate(long userId)
        {
            //check if client is connected, i.e. registered in client state
            var clientState = stateManager.GetClient(userId);
            if (clientState == null)
            {
                return false;
            }

            //get new messages to keep the client in sync
            var result = await messageService.GetNewMessages(userId, clientState.LastMessageId);
            //check if there are any new messages 
            if (result.Messages.Count()>0)
            {
                stateManager.AddUpdateClient(userId, result.Messages.Max(message => message.Id));

                string response;

                var Messages = result.Messages.ToMessageInfoResponse(userId);

                response = new ResponseBase<IEnumerable<MessageInfoBase>>
                {
                    Success = true,
                    Data = Messages
                }.ToJson();

                _ = hubContext.Clients.User(userId.ToString()).UpdateMessagesResponses(response);
            }
            return true;
        }
    }
}
