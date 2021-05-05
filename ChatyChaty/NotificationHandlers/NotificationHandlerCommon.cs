using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatyChaty.Hubs.v1;
using MediatR;

namespace ChatyChaty.NotificationHandlers
{
    /// <summary>
    /// Class that contain the common logic between notification handlers
    /// </summary>
    public abstract class NotificationHandlerCommon<T> : AsyncRequestHandler<T> where T : IRequest
    {
        protected readonly IHubHelper hubHelper;

        public NotificationHandlerCommon(IHubHelper hubHelper)
        {
            this.hubHelper = hubHelper;
        }
    }
}

