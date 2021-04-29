using Blazored.LocalStorage;
using ChatyChatyClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Repository
{
    public class LocalStorageProfileRepository : IProfileRepository
    {
        private static readonly string profileKey = "userprofile";
        private readonly ILocalStorageService localStorage;
        public LocalStorageProfileRepository(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task<UserProfile> Get()
        {
            return await localStorage.GetItemAsync<UserProfile>(profileKey);
        }

        public async Task Set(UserProfile profile)
        {
            await localStorage.SetItemAsync(profileKey, profile);
        }

        public async Task Update(UserProfile profile)
        {
            await localStorage.SetItemAsync(profileKey, profile);
        }
    }
}
