using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Entities
{
    public class Chat
    {
        public long Id { get; set; }
        public string ImgLink { get; set; }
        public string ReceiverName { get; set; }
        public IList<Message> Messages { get; set; }
    }
}
