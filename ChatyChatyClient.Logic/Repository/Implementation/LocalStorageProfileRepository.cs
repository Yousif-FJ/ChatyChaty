using Blazored.LocalStorage;
using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Repository.Implementation
{
    public class LocalStorageProfileRepository : IProfileRepository
    {
        private static readonly string profileKey = "userprofile";
        private readonly ILocalStorageService localStorage;
        public LocalStorageProfileRepository(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public ValueTask<UserProfile> Get()
        {
            return localStorage.GetItemAsync<UserProfile>(profileKey);
        }

        public ValueTask Set(UserProfile profile)
        {
            return localStorage.SetItemAsync(profileKey, profile);
        }

        public ValueTask Update(UserProfile profile)
        {
            return localStorage.SetItemAsync(profileKey, profile);
        }
    }
}
