using ChatyChaty.Domain.Model.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.Entity
{
    public class AppUser : IdentityUser<UserId>
    {
        public AppUser(string userName) : base(userName)
        {
            Id = new UserId();
        }
        public string DisplayName { get; set; }
        public ICollection<Conversation> Conversations1 { get; set; }
        public ICollection<Conversation> Conversations2 { get; set; }
        public ICollection<Message> MessageSender { get; set; }
    }

    public class Role : IdentityRole<UserId>
    {
    }

    public record UserId(string Value) {
        public UserId() : this(Guid.NewGuid().ToString()) { }
    };
}
