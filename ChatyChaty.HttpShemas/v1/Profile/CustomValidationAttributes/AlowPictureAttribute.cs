using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Profile.CustomValidationAttributes
{
    public class AlowPictureAttribute : ValidationAttribute
    {
        public const string ErrorM = "You must upload a valid Image with png, jpg or jpeg file extension";

        public AlowPictureAttribute()
        {
        }

        private static readonly Dictionary<string, List<byte[]>> _fileSignature =
        new Dictionary<string, List<byte[]>>
    {
    { ".jpeg", new List<byte[]>
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
        }
    },

    {".png", new List<byte[]>
        {
            new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
        }
    },

    { ".jpg", new List<byte[]>
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
        }
    }
    };

        protected override ValidationResult IsValid(
                object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is IFormFile file)
            {
                using var reader = new BinaryReader(file.OpenReadStream());

                var extension = Path.GetExtension(file.FileName);

                if (!_fileSignature.TryGetValue(extension, out List<byte[]> signatures))
                {
                    return new ValidationResult(ErrorM);
                }

                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                if (signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature)))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorM);
        }

    }
}
