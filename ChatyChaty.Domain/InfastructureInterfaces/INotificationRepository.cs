﻿using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    /// <summary>
    /// Interface that encapsulate logic for accessing notification data from a source
    /// </summary>
    public interface INotificationRepository
    {
        Task<Notification> GetNotificationAsync(long userId);
        Task<Notification> ResetUpdatesAsync(long userId);
        Task UsersGotChatUpdateAsync(params long[] userIds);
        Task UserGotNewMessageAsync(long userId);
        Task UsersGotMessageDeliveredAsync(params long[] userIds);
    }
}
