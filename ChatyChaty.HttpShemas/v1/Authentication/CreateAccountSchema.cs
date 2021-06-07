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
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
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
