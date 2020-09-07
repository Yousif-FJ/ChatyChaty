using ChatyChaty.Model.AccountModel;
using ChatyChaty.Services.PictureServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTest.MockClasses
{
    class MockPictureProvider : IPictureProvider
    {
        public Task<PhotoUrlModel> ChangePhoto(long UserID, string UserName, IFormFile formFile)
        {
            var tcs = new TaskCompletionSource<PhotoUrlModel>();
            try
            {
                var photoModel = new PhotoUrlModel { Success = true };
                tcs.SetResult(photoModel);
                return tcs.Task;
            }
            catch (Exception e)
            {
                tcs.SetException(e);
                return tcs.Task;
            }
        }

        public Task DeletePhoto(long UserID, string UserName)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetPhotoURL(long UserID, string UserName)
        {
            var tcs = new TaskCompletionSource<string>();
            try
            {
                tcs.SetResult(null);
                return tcs.Task;
            }
            catch (Exception e)
            {
                tcs.SetException(e);
                return tcs.Task;
            }
        }

        public string GetPlaceHolderURL()
        {
            return null;
        }
    }
}
