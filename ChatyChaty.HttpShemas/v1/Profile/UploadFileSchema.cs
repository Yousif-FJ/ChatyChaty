using ChatyChaty.HttpShemas.v1.Profile.CustomValidationAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Profile
{
    public class UploadFileSchema
    {
        [Required]
        [AlowPicture]
        [MaxFileSize(4194304)]
        public IFormFile PhotoFile { get; set; }
    }
}
