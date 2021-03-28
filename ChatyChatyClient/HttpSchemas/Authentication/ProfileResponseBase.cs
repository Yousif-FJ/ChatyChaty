using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.HttpSchemas.Authentication
{
    public class ProfileResponseBase
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string PhotoURL { get; set; }
    }
}
