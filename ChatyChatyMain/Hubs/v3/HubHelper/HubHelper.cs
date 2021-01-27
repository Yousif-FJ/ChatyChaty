using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
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

        public bool TrySendChatUpdate(long receiverId, ProfileAccountModel chatInfo)
        {
            if (hubClients.IsClientConnected(receiverId) == false)
            {
                return false;
            }

            Response<UserProfileResponseBase> response = new()
            {
                Success = true,
                Data = new UserProfileResponseBase
                {
                    Profile = new ProfileResponseBase
                    {
                        DisplayName = chatInfo.DisplayName,
                        PhotoURL = chatInfo.PhotoURL,
                        Username = chatInfo.Username
                    },
                    ChatId = chatInfo.ChatId
                }
            };

            _ = hubContext.Clients.User(receiverId.ToString()).UpdateChat(response);
            return true;
        }

        public bool TrySendMessageStatusUpdate(long receiverId, long chatId, long messageId)
        {
            if (hubClients.IsClientConnected(receiverId) == false)
            {
                return false;
            }

            var response = new Response<MessageStatusResponseBase>()
            {
                Success = true,
                Data = new MessageStatusResponseBase(messageId, chatId, true)
            };

            _ = hubContext.Clients.User(receiverId.ToString()).UpdateMessageStatus(response);

            return true;
        }

        /// <summary>
        /// Method called to send message updates to a connected client, return false if the client is not connected
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool TrySendMessageUpdate(long receiverId, IEnumerable<Message> messages)
        {
            if (hubClients.IsClientConnected(receiverId) == false)
            {
                return false;
            }

            if (messages.Any())
            {
                IEnumerable<MessageInfoReponseBase> messagesInfo = messages.ToMessageInfoResponse(receiverId);


                var response = new Response<IEnumerable<MessageInfoReponseBase>>
                {
                    Success = true,
                    Data = messagesInfo
                };

                //send response to clients
                _ = hubContext.Clients.User(receiverId.ToString()).UpdateMessages(response);
            }

            return true;
        }
    }
}
