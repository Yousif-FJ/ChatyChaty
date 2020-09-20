using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Model.MessagingModel;
using ChatyChaty.Services.MessageServices;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    /// <summary>
    /// A class that contain common logic between MainHub and HubHelper
    /// </summary>
    public static class HubExtensionMethods
    {
        /// <summary>
        /// Take messages from messages model and send them to clients
        /// </summary>
        /// <param name="hubClients"></param>
        /// <param name="newMessagesModel"></param>
        /// <param name="userId"></param>
        /// <param name="lastMessageId"></param>
        /// <returns>Return last messageId after update</returns>
        static public void SendMessageUpdatesAsync(this IHubClients<IChatClient> hubClients, GetNewMessagesModel newMessagesModel,
            long userId, ref long lastMessageId)
        {
            //if there are new messages
            if (newMessagesModel.Messages.Count() > 0)
            {
                //convert from model to response class
                var messages = newMessagesModel.Messages.ToMessageInfoResponse(userId);

                //create response
                var response = new ResponseBase<IEnumerable<MessageInfoBase>>
                {
                    Success = true,
                    Data = messages
                }.ToJson();

                //send response to clients
                _ = hubClients.User(userId.ToString()).UpdateMessagesResponses(response);
                //update last messageId
                lastMessageId = messages.Max(m => m.MessageId);
            }
        }
    }
}
