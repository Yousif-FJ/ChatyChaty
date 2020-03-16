using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.ValidationAttributes;

namespace ChatyChaty.ControllerSchema.v1
{
    public class UploadFileSchema
    {
        [Required]
        [AlowPicture]
        public IFormFile PhotoFile { get; set; }
    }
}
