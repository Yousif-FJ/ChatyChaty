using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class Message
    {
        [Required]
        public long ID { get; set; }
        [Required]
        public string Sender { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
