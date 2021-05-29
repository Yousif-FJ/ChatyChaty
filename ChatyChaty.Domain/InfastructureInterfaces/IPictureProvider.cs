using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
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
        public Task<string> ChangePhoto(UserId userID,string fileName, Stream file);
        public Task DeletePhoto(UserId userID);
    }
}
