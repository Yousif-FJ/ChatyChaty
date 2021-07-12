using Blazored.LocalStorage;
using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.RepositoryImplementation
{
    public class LocalMyProfileRepository : ISelfProfileRepository
    {
        private static readonly string profileKey = "userprofile";
        private readonly ILocalStorageService localStorage;
        public LocalMyProfileRepository(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public ValueTask<UserProfile> Get()
        {
            return localStorage.GetItemAsync<UserProfile>(profileKey);
        }

        public ValueTask Remove()
        {
            return localStorage.RemoveItemAsync(profileKey);
        }

        public ValueTask Set(UserProfile profile)
        {
            if (profile is null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return localStorage.SetItemAsync(profileKey, profile);
        }

        public ValueTask Update(UserProfile profile)
        {
            if (profile is null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return localStorage.SetItemAsync(profileKey, profile);
        }
    }
}
