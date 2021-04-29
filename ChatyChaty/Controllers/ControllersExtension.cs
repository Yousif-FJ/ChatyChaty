using ChatyChaty.Domain.ApplicationExceptions;
using ChatyChaty.Domain.Model.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatyChaty.Controllers
{
    public static class ControllersExtension
    {
        public static UserId GetUserIdFromHeader(this HttpContext context)
        {
            var IdClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            return new UserId(IdClaim.Value);
        }
    }
}
