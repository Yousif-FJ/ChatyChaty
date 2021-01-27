using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.MessagingModel
{
    public class SendMessageModel
    {
        public Message Message { get; set; }
        public string Error { get; set; }
    }
}
