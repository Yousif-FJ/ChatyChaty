﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.DBModel
{
    public class AppUser : IdentityUser<long>
    {
        public AppUser(string userName) : base(userName)
        {
        }
        public string DisplayName { get; set; }
        public ICollection<Chat> Chat1 { get; set; }
        public ICollection<Chat> Chat2 { get; set; }
        public ICollection<Message> MessageSender { get; set; }
    }

    public class Role : IdentityRole<long>
    {
    }
}
