using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class AppUser : IdentityUser<long>
    {
        public AppUser(string userName) : base(userName)
        {
        }
        public override long Id { get => base.Id; set => base.Id = value; }
    }

    public class Role : IdentityRole<long>
    {
    }
}
