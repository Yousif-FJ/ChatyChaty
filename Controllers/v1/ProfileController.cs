using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v1;
using ChatyChaty.Model.FileRepository;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IFileRepository fileRepository;
        private readonly IAccountManager accountManager;

        public ProfileController(IFileRepository fileRepository, IAccountManager accountManager)
        {
            this.fileRepository = fileRepository;
            this.accountManager = accountManager;
        }

        /// <summary>
        /// Takes a UserName and gives a Photo Location
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        /// <response code="404">The given UserName doesn't exist</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetUserPhoto")]
        public async Task<IActionResult> GetUserPhoto([FromQuery]string UserName)
        {
            var User = await accountManager.GetUser(UserName);
            string UserPhotoName = "Placeholder.jpg";
            if (User is null)
            {
                return NotFound();
            }
            if (User.PhotoName != null)
            {
                UserPhotoName = User.PhotoName;
            }
            return Ok($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/ProfilePIctures/{UserPhotoName}");
        }

        /// <summary>
        /// set photo picture or replace existing one (Require authentication)
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("SetPhotoForSelf")]
        public async Task<IActionResult> SetPhotoForSelf([FromBody]UploadFileSchema uploadFile)
        {
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            var user = await accountManager.GetUser(UserNameClaim.Value);
            if (user.PhotoName is null)
            {
                var NewFileName = await fileRepository.UploadPhoto(uploadFile.PhotoFile);
                await accountManager.SetPhotoName(user.UserName, NewFileName);
            }
            else
            {
                var FileName = await fileRepository.ChangePhoto(user.PhotoName, uploadFile.PhotoFile);
            }
                return Ok();
        }
    }
}