using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Authentication
{
    public class LoginAccountSchema
    {
        public LoginAccountSchema(string username, string password)
        {
            Username = username;
            Password = password;
        }

        [Required]
        [MaxLength(32)]
        public string Username { get; set; }
        [Required]
        [MaxLength(64)]
        public string Password { get; set; }
    }
}
