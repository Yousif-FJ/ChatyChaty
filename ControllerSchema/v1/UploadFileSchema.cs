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
        [MaxFileSize(512000)]
        [AlowPicture]
        public IFormFile PhotoFile { get; set; }
    }
}
