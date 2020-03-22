﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v1.Profile
{
    public class GetUserProfileResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public long? ConversationId { get; set; }
        public string PictureUrl { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
    }
}
