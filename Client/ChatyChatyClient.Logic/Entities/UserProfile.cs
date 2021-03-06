﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Entities
{
    public class UserProfile
    {
        public UserProfile(string username, string displayName, string photoURL = null)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or whitespace", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException($"'{nameof(displayName)}' cannot be null or whitespace", nameof(displayName));
            }

            Username = username;
            DisplayName = displayName;
            PhotoURL = photoURL;
        }
        public string Username { get; init; }
        public string DisplayName { get; init; }
        public string PhotoURL { get; set; }
    }
}
