using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v1
{
    public class MessageSchema
    {
        [Required]
        public string Sender { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
