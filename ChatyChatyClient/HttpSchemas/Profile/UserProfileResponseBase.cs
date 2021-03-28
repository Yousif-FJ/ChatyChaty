using ChatyChatyClient.HttpSchemas.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.HttpSchemas.Profile
{
    public class UserProfileResponseBase
    {
        public string ChatId { get; set; }
        public ProfileResponseBase Profile { get; set; }
    }
}
