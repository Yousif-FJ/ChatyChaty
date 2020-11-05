using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTest.MockClasses
{
    class MockPictureProvider : IPictureProvider
    {
        public Task<PhotoUrlModel> ChangePhoto(long userID, string userName, string fileName, Stream file)
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
            return null;
        }

        public string GetPlaceHolderURL()
        {
            return null;
        }
    }
}
