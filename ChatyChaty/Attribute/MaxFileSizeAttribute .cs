using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.ValidationAttributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        private string MaxFileSizeFormated
        {
            get
            {
               return BytesToString(_maxFileSize);
            }
        }
        public MaxFileSizeAttribute(int maxFileSizeBytes)
        {
            _maxFileSize = maxFileSizeBytes;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is IFormFile file)
            {
                if (file.Length < _maxFileSize)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(ErrorM());
        }

        private static string BytesToString(int byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB" };
            if (byteCount == 0)
                return "0" + suf[0];
            int bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return $"{(Math.Sign(byteCount) * num)}{suf[place]}";
        }

        public string ErrorM()
        {
            return $"Maximum allowed size is {MaxFileSizeFormated}";
        }
    }
}
