using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v2
{
    public class CheckForUpdatesResponse
    {
        public bool ChatUpdate { get; set; }
        public bool MessageUpdate { get; set; }
        public bool DeliveredUpdate { get; set; }
    }
}
