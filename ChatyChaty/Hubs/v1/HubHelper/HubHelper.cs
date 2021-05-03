using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.HttpShemas.v1.Message;
using ChatyChaty.HttpShemas.v1.Profile;
using ChatyChaty.ModelExtensions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v1
{
    /// <summary>
    /// Used to send real time update using signalR
    /// </summary>
    public class HubHelper : IHubHelper
    {
        private readonly IHubContext<MainHub, IChatClient> hubContext;
        private readonly IHubSessions hubClients;

        public HubHelper(IHubContext<MainHub, IChatClient> hubContext,
            IHubSessions hubClients
            )
        {
            this.hubContext = hubContext;
            this.hubClients = hubClients;
        }

        public bool TrySendChatUpdate(UserId receiverId, ProfileAccountModel chatInfo)
        {
            if (hubClients.IsClientConnected(receiverId) == false)
            {
                return false;
            }

            var response = new UserProfileResponse(
                chatInfo.ChatId.Value,
                new ProfileResponse(
                    chatInfo.Username,
                    chatInfo.DisplayName,
                    chatInfo.PhotoURL)
            );

            _ = hubContext.Clients.User(receiverId.ToString()).UpdateChat(response);
            return true;
        }

        public bool TrySendMessageStatusUpdate(UserId receiverId, ConversationId chatId, MessageId messageId)
        {
            if (hubClients.IsClientConnected(receiverId) == false)
            {
                return false;
            }

            var response =  new MessageStatusResponse(messageId.Value, chatId.Value, true);

            _ = hubContext.Clients.User(receiverId.ToString()).UpdateMessageStatus(response);

            return true;
        }

        /// <summary>
        /// Method called to send message updates to a connected client, return false if the client is not connected
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool TrySendMessageUpdate(UserId receiverId, IEnumerable<Message> messages)
        {
            if (hubClients.IsClientConnected(receiverId) == false)
            {
                return false;
            }

            if (messages.Any())
            {
                IEnumerable<MessageInfoReponse> response = messages.ToMessageInfoResponse(receiverId);

                //send response to clients
                _ = hubContext.Clients.User(receiverId.ToString()).UpdateMessages(response);
            }

            return true;
        }
    }
}
