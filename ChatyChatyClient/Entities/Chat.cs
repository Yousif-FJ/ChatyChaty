using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string ImgLink { get; set; }
        public string ReceiverName { get; set; }
        public bool IsThereNewMessage { get; set; }
    }
}
