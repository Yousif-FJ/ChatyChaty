using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.MessageServices;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    //TO-Do: extract the check logic into separate method
    /// <summary>
    /// This class is used when the hub is called outside of the MainHub classes
    /// </summary>
    public class HubHelper : IHubHelper
    {
        private readonly IHubContext<MainHub, IChatClient> hubContext;
        private readonly IMessageService messageService;
        private readonly IAccountManager accountManager;
        private readonly IHubSessions hubClients;

        public HubHelper(IHubContext<MainHub, IChatClient> hubContext,
            IMessageService messageService,
            IAccountManager accountManager,
            IHubSessions hubClients
            )
        {
            this.hubContext = hubContext;
            this.messageService = messageService;
            this.accountManager = accountManager;
            this.hubClients = hubClients;
        }

        public async Task<bool> SendChatUpdateAsync(long receiverId, long chatId)
        {
            var IsConnected = hubClients.IsClientConnected(receiverId);
            if (IsConnected == false)
            {
                return false;
            }

            var result = await accountManager.GetConversation(chatId, receiverId);


            Response<UserProfileResponseBase> response = new()
            {
                Success = true,
                Data = new UserProfileResponseBase
                {
                    Profile = new ProfileSchema
                    {
                        DisplayName = result.DisplayName,
                        PhotoURL = result.PhotoURL,
                        Username = result.Username
                    },
                    ChatId = chatId
                }
            };

            _ = hubContext.Clients.User(receiverId.ToString()).UpdateChatResponse(response);
            return true;
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
            if (result.Messages.Any())
            {
                //convert from model to response class
                var messages = result.Messages.ToMessageInfoResponse(userId);

                //create response
                var response = new Response<IEnumerable<MessageInfoBase>>
                {
                    Success = true,
                    Data = messages
                };

                //send response to clients
                _ = hubContext.Clients.User(userId.ToString()).UpdateMessagesResponse(response);
            }

            return true;
        }
    }
}
