using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
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
        public Task<PhotoUrlModel> ChangePhoto(UserId userID, string userName, string fileName, Stream file)
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

        public Task DeletePhoto(UserId userID, string UserName)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetPhotoURL(UserId userID, string UserName)
        {
            return null;
        }

        public string GetPlaceHolderURL()
        {
            return null;
        }
    }
}
