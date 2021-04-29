using ChatyChaty.ControllerHubSchema.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ValidationAttribute
{
    public class CustomModelValidationResponseAttribute : ActionFilterAttribute
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid == false)
            {
                var errors = context.ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
                context.Result = new BadRequestObjectResult(new ErrorResponse(errors));
            }
            else
            {
                await next();
            }
        }
    }
}
