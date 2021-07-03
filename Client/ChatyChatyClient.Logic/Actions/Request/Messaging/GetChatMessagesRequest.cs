using ChatyChatyClient.Logic.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Actions.Request.Messaging
{
    public class GetChatMessagesRequest : IRequest<Chat>
    {
        public string ChatId { get;}
        public GetChatMessagesRequest(string chatId)
        {
            ChatId = chatId;
        }
    }
}
