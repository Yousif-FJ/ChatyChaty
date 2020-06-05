﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.AccountModel
{
    public class ProfileAccountModel
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string PhotoURL { get; set; }
        public long? UserId { get; set; }
    }
}
