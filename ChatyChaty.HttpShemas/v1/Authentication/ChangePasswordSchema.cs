﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Authentication
{
    public class ChangePasswordSchema
    {
        public ChangePasswordSchema(string currentPassword, string newPassword)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }

        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [MaxLength(64)]
        public string NewPassword { get; set; }
    }
}
