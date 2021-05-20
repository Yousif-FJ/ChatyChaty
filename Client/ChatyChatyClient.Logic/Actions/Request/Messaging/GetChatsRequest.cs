using ChatyChatyClient.Logic.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Actions.Request.Messaging
{
    public class GetChatsRequest : IRequest<IList<Chat>>
    {
    }
}
