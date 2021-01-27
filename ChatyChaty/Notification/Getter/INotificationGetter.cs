using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.NotficationServices.Getter
{
    public interface INotificationGetter
    {
        Task<Notification> CheckForUpdatesAsync(long userId);
    }
}
