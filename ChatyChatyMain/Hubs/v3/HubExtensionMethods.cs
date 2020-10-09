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
        /// <returns>Return last messageId after update</returns>
        static public void SendMessageUpdates(this IHubClients<IChatClient> hubClients, GetNewMessagesModel newMessagesModel, long userId)
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
            }
        }

        static public void SendMessageUpdates(this IHubClients<IChatClient> hubClients, SendMessageModel MessagesModel,
            long userId)
        {
            if (MessagesModel != null)
            {
                //convert from model to response class
                var message = new MessageInfoBase(MessagesModel.Message, userId);

                //create response
                var response = new ResponseBase<IEnumerable<MessageInfoBase>>
                {
                    Success = true,
                    Data = new List<MessageInfoBase> { message }
                }.ToJson();

                //send response to clients
                _ = hubClients.User(userId.ToString()).UpdateMessagesResponses(response);
            }
        }
    }
}
