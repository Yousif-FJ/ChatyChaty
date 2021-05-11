using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatyChatyClient.Logic.Actions.Request.Messaging;
using ChatyChatyClient.Logic.Entities;
using MediatR;

namespace ChatyChatyClient.Logic.Actions.Handler.Messaging
{
    public class GetChatsHandler : IRequestHandler<GetChatsRequest, IList<Chat>>
    {
        public Task<IList<Chat>> Handle(GetChatsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
