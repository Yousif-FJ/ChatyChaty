using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class ChatyChatyContext:DbContext
    {
        public DbSet<Message> MessagesSet { get; set; }
    }
}

