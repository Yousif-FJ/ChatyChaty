using ChatyChaty.Domain.Model.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatyChaty.Domain.Model.Entity
{
    public class AppUser : IdentityUser<UserId>
    {
        public AppUser(string userName) : base(userName)
        {
            Id = new UserId();
            DisplayName = userName;
        }
        public AppUser(string userName, string displayName) : base(userName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException($"'{nameof(displayName)}' cannot be null or whitespace", nameof(displayName));
            }

            Id = new UserId();
            DisplayName = displayName;
        }
        public string DisplayName { get; private set; }
        public List<Conversation> Conversations1 { get; private set; }
        public List<Conversation> Conversations2 { get; private set; }
        public List<Message> MessageSender { get; private set; }

        public void ChangeDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException($"'{nameof(displayName)}' cannot be null or whitespace", nameof(displayName));
            }
            this.DisplayName = displayName;
        }
    }

    public class Role : IdentityRole<UserId>
    {
    }

    public record UserId(string Value) {
        public UserId() : this(Guid.NewGuid().ToString()) { }
        
        public override string ToString()
        {
            return Value;
        }
    };
}
