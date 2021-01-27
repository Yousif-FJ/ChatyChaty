﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public class ChangePasswordSchema
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [MaxLength(64)]
        public string NewPassword { get; set; }
    }
}