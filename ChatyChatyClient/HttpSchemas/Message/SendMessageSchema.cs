using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.HttpSchemas.Message
{
    public class SendMessageSchema
    {
        [Required]
        public string ChatId { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
