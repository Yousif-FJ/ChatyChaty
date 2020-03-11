using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class AppUser : IdentityUser
    {
        public AppUser(string userName) : base(userName)
        {
        }

        public string PhotoName { get; set; }
    }
}
