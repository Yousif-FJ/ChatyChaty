using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v2
{
    public class LoginAccountSchema
    {
        [Required]
        [MaxLength(32)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
