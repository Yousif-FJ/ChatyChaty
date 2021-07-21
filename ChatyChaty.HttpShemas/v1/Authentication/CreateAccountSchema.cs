using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Authentication
{
    public class CreateAccountSchema
    {
        public CreateAccountSchema(string username, string password, string displayName)
        {
            Username = username;
            Password = password;
            DisplayName = displayName;
        }

        [Required]
        [MaxLength(32)]
        public string Username { get; set; }
        [Required]
        [MaxLength(64)]
        public string Password { get; set; }
        [Required]
        [MaxLength(32)]
        public string DisplayName { get; set; }
    }
}
