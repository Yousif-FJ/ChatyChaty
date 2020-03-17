using ChatyChaty.Model.DB_Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class AppUser : IdentityUser<long>
    {
        public AppUser(string userName) : base(userName)
        {
        }
        public string DisplayName { get; set; }
        public ICollection<Conversation> Conversations1 { get; set; }
        public ICollection<Conversation> Conversations2 { get; set; }
        public ICollection<Message> MessageSender { get; set; }
    }

    public class Role : IdentityRole<long>
    {
    }
}
