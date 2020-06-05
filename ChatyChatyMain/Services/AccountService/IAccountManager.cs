﻿using ChatyChaty.Model;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public interface IAccountManager
    {
        Task<ProfileAccountModel> GetUser(string UserName);
        Task<string> UpdateDisplayName(long UserId, string NewDisplayName);
        Task<PhotoUrlModel> SetPhoto(long UserId, IFormFile formFile);
    }
}
