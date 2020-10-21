using ChatyChaty.Domain.Model.AccountModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    /// <summary>
    /// A class that manage uploading and accessing Photos stored in a Provider
    /// </summary>
    public interface IPictureProvider
    {
        public string GetPlaceHolderURL();
        public Task<PhotoUrlModel> ChangePhoto(long userID, string userName,string fileName, Stream file);
        public Task<string> GetPhotoURL(long userID, string userName);
        public Task DeletePhoto(long userID, string userName);
    }
}
