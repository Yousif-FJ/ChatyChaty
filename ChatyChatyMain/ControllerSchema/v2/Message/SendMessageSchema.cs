using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v2
{
    public class SendMessageSchema
    {
        [Required]
        public long ChatId { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
