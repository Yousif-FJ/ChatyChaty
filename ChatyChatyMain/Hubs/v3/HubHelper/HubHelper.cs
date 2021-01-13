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

        public async Task<bool> TrySendChatUpdateAsync(long receiverId, long chatId)
        {
            if (hubClients.IsClientConnected(receiverId) == false)
            {
                return false;
            }

            var result = await accountManager.GetConversation(chatId, receiverId);

            Response<UserProfileResponseBase> response = new()
            {
                Success = true,
                Data = new UserProfileResponseBase
                {
                    Profile = new ProfileResponseBase
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

        public async Task<bool> TrySendMessageStatusUpdateAsync(long receiverId, long messageId)
        {
            if (hubClients.IsClientConnected(receiverId) == false)
            {
                return false;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Method called to send message updates to a connected client, return false if the client is not connected
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="lastMessageId"></param>
        /// <returns></returns>
        public async Task<bool> TrySendMessageUpdateAsync(long userId, long lastMessageId)
        {
            if (hubClients.IsClientConnected(userId) == false)
            {
                return false;
            }

            var result = await messageService.GetNewMessages(userId, lastMessageId-1);

            if (result.Messages.Any())
            {
                //convert from model to response class
                var messages = result.Messages.ToMessageInfoResponse(userId);

                //create response
                var response = new Response<IEnumerable<MessageInfoReponseBase>>
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
