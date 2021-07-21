using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Message
{
    public class SendMessageSchema
    {
        public SendMessageSchema(string chatId, string body)
        {
            ChatId = chatId;
            Body = body;
        }

        [Required]
        public string ChatId { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
