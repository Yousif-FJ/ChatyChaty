﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.DBModel
{
    public class Conversation
    {
        [Key]
        public long Id { get; set; }
        public AppUser FirstUser { get; set; }
        public long FirstUserId { get; set; }
        public AppUser SecondUser { get; set; }
        public long SecondUserId { get; set; }
        public ICollection<Message> Messages { get; set; }

    }
}