using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.DBModel
{
    public class Notification
    {
        public long Id { get; set; }
        public bool ChatUpdate { get; set; }
        public bool MessageUpdate { get; set; }
        public bool DeliveredUpdate { get; set; }
        public long UserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
