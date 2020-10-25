using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.Entity
{
    public class Notification
    {
        public Notification(long userId)
        {
            UserId = userId;
        }
        public long Id { get; private set; }
        public bool ChatUpdate { get; private set; }
        public bool MessageUpdate { get; private set; }
        public bool DeliveredUpdate { get; private set; }
        public long UserId { get; private set; }
        public AppUser AppUser { get; set; }

        public void Reset()
        {
            ChatUpdate = false;
            MessageUpdate = false;
            DeliveredUpdate = false;
        }

        public void GotChatUpdate()
        {
            ChatUpdate = true;
        }

        public void GotMessageUpdate()
        {
            MessageUpdate = true;
        }

        public void GotDeliveredUpdate()
        {
            DeliveredUpdate = true;
        }
    }
}
