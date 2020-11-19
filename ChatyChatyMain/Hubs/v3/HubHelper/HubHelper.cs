using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Domain.Services.MessageServices;
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
    public class HubHelper : IHubHelper
    {
        private readonly IHubContext<MainHub, IChatClient> hubContext;
        private readonly IMessageService messageService;
        private readonly HubConnectedClients hubClients;

        public HubHelper(IHubContext<MainHub, IChatClient> hubContext,
            IMessageService messageService,
            HubConnectedClients hubClients
            )
        {
            this.hubContext = hubContext;
            this.messageService = messageService;
            this.hubClients = hubClients;
        }

        public Task<bool> SendChatUpdateAsync(long receiverId, long chatId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendMessageStatusUpdateAsync(long userId, long messageId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method called to send message updates to a connected client, return false if the client is not connected
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="lastMessageId"></param>
        /// <returns></returns>
        public async Task<bool> SendMessageUpdateAsync(long userId, long lastMessageId)
        {
            //check if client is connecte
            var IsConnected = hubClients.IsClientConnected(userId);
            if (IsConnected == false)
            {
                return false;
            }

            //get new messages from message service
            var result = await messageService.GetNewMessages(userId, lastMessageId-1);

            //hubContext.Clients.SendMessageUpdates(result, userId);

            //if there are new messages
            if (result.Messages.Count() > 0)
            {
                //convert from model to response class
                var messages = result.Messages.ToMessageInfoResponse(userId);

                //create response
                var response = new ResponseBase<IEnumerable<MessageInfoBase>>
                {
                    Success = true,
                    Data = messages
                };

                //send response to clients
                _ = hubContext.Clients.User(userId.ToString()).UpdateMessagesResponses(response);
            }

            return true;
        }
    }
}
