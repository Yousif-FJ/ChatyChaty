using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public class CheckForUpdatesResponseBase
    {
        public bool ChatUpdate { get; set; }
        public bool MessageUpdate { get; set; }
        public bool DeliveredUpdate { get; set; }
    }
}
