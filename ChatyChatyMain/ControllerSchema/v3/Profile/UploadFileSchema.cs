using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.ValidationAttributes;

namespace ChatyChaty.ControllerSchema.v3
{
    public class UploadFileSchema
    {
        [Required]
        [AlowPicture]
        [MaxFileSize(4194304)]
        public IFormFile PhotoFile { get; set; }
    }
}
