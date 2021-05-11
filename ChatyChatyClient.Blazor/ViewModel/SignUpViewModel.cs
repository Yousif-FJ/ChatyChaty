using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.ViewModel
{
    public class SignUpViewModel
    {
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
